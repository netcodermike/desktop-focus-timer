using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Text.Json;
using System.IO;

public sealed class Session {
 public int FocusMinutes{get;set;}=25;
 public int BreakMinutes{get;set;}=5;
 public int Completed{get;set;}
 public bool IsBreak{get;private set;}
 public bool Running{get;private set;}
 public TimeSpan Remaining{get;private set;}=TimeSpan.FromMinutes(25);
 private DateTimeOffset? deadline;
 public void Start(DateTimeOffset now){if(!Running){deadline=now+Remaining;Running=true;}}
 public void Pause(DateTimeOffset now){Tick(now);Running=false;deadline=null;}
 public void Reset(){Running=false;deadline=null;Remaining=TimeSpan.FromMinutes(IsBreak?BreakMinutes:FocusMinutes);}
 public bool Tick(DateTimeOffset now){
  if(!Running||deadline is null)return false;
  Remaining=deadline.Value-now;
  if(Remaining>TimeSpan.Zero)return false;
  if(!IsBreak)Completed++;
  IsBreak=!IsBreak;Running=false;deadline=null;Remaining=TimeSpan.FromMinutes(IsBreak?BreakMinutes:FocusMinutes);return true;
 }
}
class Program {
 [STAThread] public static int Main(string[] args){
  if(args.Contains("--self-test")){
   var s=new Session();var now=DateTimeOffset.UnixEpoch;s.Start(now);s.Pause(now.AddMinutes(10));if(s.Remaining!=TimeSpan.FromMinutes(15))return 1;
   s.Start(now.AddHours(1));if(!s.Tick(now.AddHours(1).AddMinutes(15))||!s.IsBreak||s.Completed!=1||s.Running)return 1;
   s.Reset();if(s.Remaining!=TimeSpan.FromMinutes(5))return 1;
   var copy=JsonSerializer.Deserialize<Session>(JsonSerializer.Serialize(s));if(copy?.Completed!=1)return 1;return 0;
  }
  var path=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"NetCoderMike","FocusTimer","settings.json");
  Session session;
  try{session=File.Exists(path)?JsonSerializer.Deserialize<Session>(File.ReadAllText(path))??new():new();if(session.FocusMinutes is <1 or >180||session.BreakMinutes is <1 or >60||session.Completed<0)throw new InvalidDataException("Invalid settings");session.Reset();}
  catch(Exception e)when(e is IOException or JsonException or InvalidDataException or UnauthorizedAccessException){MessageBox.Show("Cannot read saved settings: "+e.Message);return 1;}
  var app=new Application();var window=new Window{Title="Focus Timer",Width=480,Height=520,MinWidth=420,MinHeight=480,WindowStartupLocation=WindowStartupLocation.CenterScreen};
  var panel=new StackPanel{Margin=new Thickness(28)};window.Content=panel;
  var phase=new TextBlock{FontSize=26};var clock=new TextBlock{FontSize=72,Margin=new Thickness(0,20,0,10)};var total=new TextBlock();
  panel.Children.Add(phase);panel.Children.Add(clock);panel.Children.Add(total);
  var focus=new TextBox{Text=session.FocusMinutes.ToString(),Margin=new Thickness(0,6,0,10)};var rest=new TextBox{Text=session.BreakMinutes.ToString(),Margin=new Thickness(0,6,0,10)};
  panel.Children.Add(new TextBlock{Text="Focus minutes (1–180)"});panel.Children.Add(focus);panel.Children.Add(new TextBlock{Text="Break minutes (1–60)"});panel.Children.Add(rest);
  void Save(){try{Directory.CreateDirectory(Path.GetDirectoryName(path)!);var tmp=path+".tmp";File.WriteAllText(tmp,JsonSerializer.Serialize(session));File.Move(tmp,path,true);}catch(IOException e){MessageBox.Show("Save failed: "+e.Message);}}
  void Draw(){phase.Text=session.IsBreak?"Take a break":"Time to focus";clock.Text=$"{(int)session.Remaining.TotalMinutes:00}:{session.Remaining.Seconds:00}";total.Text=$"Completed focus sessions: {session.Completed}";focus.IsEnabled=rest.IsEnabled=!session.Running;}
  void Button(string text,Action action){var b=new Button{Content=text,Padding=new Thickness(12,7,12,7),Margin=new Thickness(0,8,0,0)};b.Click+=(_,_)=>{action();Draw();};panel.Children.Add(b);}
  Button("Start / resume",()=>session.Start(DateTimeOffset.UtcNow));
  Button("Pause",()=>session.Pause(DateTimeOffset.UtcNow));
  Button("Apply durations and reset",()=>{if(!int.TryParse(focus.Text,out var f)||f is <1 or >180||!int.TryParse(rest.Text,out var b)||b is <1 or >60){MessageBox.Show("Enter whole minutes in the displayed ranges.");return;}session.FocusMinutes=f;session.BreakMinutes=b;session.Reset();Save();});
  var timer=new DispatcherTimer{Interval=TimeSpan.FromMilliseconds(200)};timer.Tick+=(_,_)=>{if(session.Tick(DateTimeOffset.UtcNow)){Save();System.Media.SystemSounds.Asterisk.Play();window.Activate();}Draw();};timer.Start();window.Closed+=(_,_)=>{timer.Stop();Save();};Draw();app.Run(window);return 0;
 }
}
