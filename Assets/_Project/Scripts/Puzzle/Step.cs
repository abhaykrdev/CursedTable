namespace HollowSeven {
public readonly struct Step {
    public readonly int A,B,Result; public readonly OpKind Op;
    public Step(int a,int b,OpKind op,int result) { A=a; B=b; Op=op; Result=result; }
    public override string ToString()=>A+" "+Op+" "+B+" = "+Result;
}
}
