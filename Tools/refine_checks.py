from pathlib import Path
p=Path('Assets/_Project/Scripts/Editor/RuntimeSmokeTest.cs')
s=p.read_text(encoding='utf-8')
a=s.index('        g.StartTutorial();')
b=s.index('        SaveService.Set("ln_beatsSeen",0);',a)
s=s[:a]+'''        g.StartTutorial();yield return null;Check(!g.Hud.Hint.gameObject.activeSelf&&!g.Hud.Timer.gameObject.activeSelf,"First hand hides advanced controls and timer");
        Click(g,0,OpKind.Add,1);yield return Wait(()=>g.Round.Puzzle.Target==32);yield return null;
        Click(g,1,OpKind.Mul,2);yield return Wait(()=>!g.Round.Busy);yield return null;Click(g,0,OpKind.Add,1);yield return Wait(()=>g.Round.Puzzle.Target==37);yield return null;
        Click(g,0,OpKind.Mul,1);yield return Wait(()=>!g.Round.Busy);yield return null;Click(g,0,OpKind.Add,2);yield return Wait(()=>g.State==GameState.RoundActive);
        Check(SaveService.TutorialDone&&g.Luck.Luck==5,"Three guided hands complete; first run starts at Luck 5");
        Check(g.Hud.Timer.gameObject.activeSelf&&g.Hud.Hint.gameObject.activeSelf,"Full controls appear after guided hands");
''' + s[b:]
s=s.replace('var routine=Tests();bool failed=false;while(true){object current=null;bool next=false;try{next=routine.MoveNext();if(next)current=routine.Current;}catch(Exception e){Results.Add("FAIL: "+e);failed=true;}if(!next||failed)break;yield return current;}', 'var stack=new Stack<IEnumerator>();stack.Push(Tests());bool failed=false;while(stack.Count>0&&!failed){object current=null;bool next=false;try{next=stack.Peek().MoveNext();if(next)current=stack.Peek().Current;}catch(Exception e){Results.Add("FAIL: "+e);failed=true;}if(failed)break;if(!next){stack.Pop();continue;}if(current is IEnumerator nested){stack.Push(nested);continue;}yield return current;}')
s=s.replace('Results.Add("PASS: "+message);','Results.Add("PASS: "+message);File.WriteAllText("Docs/runtime-progress.txt",string.Join("\\n",Results));')
p.write_text(s,encoding='utf-8')
p=Path('Assets/_Project/Scripts/UI/HudController.cs')
s=p.read_text(encoding='utf-8').replace('"SKIP TUTORIAL",-810,176,220,56','"SKIP",-850,176,140,48').replace('new Color(.016f,.014f,.022f,.98f)','new Color(.016f,.014f,.022f,1f)')
s=s.replace('chip.Glow=UIFactory.Shape', '''for(int mark=0;mark<8;mark++){float angle=mark*Mathf.PI/4;var inlay=UIFactory.Panel(disc.transform,"Inlay_"+mark,Mathf.Cos(angle)*81,Mathf.Sin(angle)*81,6,15,UIFactory.Hex("B8A27B"));inlay.rectTransform.localRotation=Quaternion.Euler(0,0,mark*45-90);}chip.Glow=UIFactory.Shape''')
s=s.replace('chip.Number.enableAutoSizing=true;','chip.Number.fontStyle=FontStyles.Bold;chip.Number.enableAutoSizing=true;')
p.write_text(s,encoding='utf-8')
