namespace HollowSeven {
public static class Operation {
    public static int ValueCap=9999;
    public static bool TryApply(int x,int y,OpKind op,out int result,out int left,out int right) {
        left=x; right=y; result=0; if(x<1||y<1) return false;
        if(op==OpKind.Sub||op==OpKind.Div) {left=System.Math.Max(x,y);right=System.Math.Min(x,y);}
        long r;
        switch(op) {
            case OpKind.Add:r=(long)x+y;break;
            case OpKind.Sub:r=left-right;break;
            case OpKind.Mul:if(x==1||y==1)return false;r=(long)x*y;break;
            case OpKind.Div:if(right==1||left%right!=0)return false;r=left/right;break;
            default:return false;
        }
        if(r<1||r>ValueCap)return false;result=(int)r;return true;
    }
}
}
