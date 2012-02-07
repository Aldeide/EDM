Public Class Form1
    Dim kv, kv2 As Double()
    Dim nd, nd2 As Double(,)
    Dim Anode As Nurbs
    Dim Cathode As Nurbs
    Dim Circle As Nurbs
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        ' Anode = New Nurbs(nd, kv)
        'Anode.Refine(0.01, 0.99, 500)
        ' Anode.DrawCurve()
        ' Anode.DrawHull()
        ' Anode.DrawCP()
        '  Label1.Text = Anode.PNumKnots
        '  Label2.Text = Anode.PNumCP

        '  Cathode = New Nurbs(nd2, kv2)
        ' Cathode.PNurbsDistance = Anode
        ' Cathode.Refine(0.01, 0.98999999999999999, 2000)
        '  Cathode.DrawCurve()


    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReDim Preserve kv(8)

        kv(0) = 0
        kv(1) = 0
        kv(2) = 0
        kv(3) = 0.25
        kv(4) = 0.5
        kv(5) = 0.75
        kv(6) = 1
        kv(7) = 1
        kv(8) = 1



        ReDim Preserve nd(2, 4)

        nd(0, 0) = 300
        nd(1, 0) = 50
        nd(2, 0) = 1
        nd(0, 1) = 300
        nd(1, 1) = 250
        nd(2, 1) = 1
        nd(0, 2) = 400 * 1 / Math.Sqrt(2)
        nd(1, 2) = 350 * 1 / Math.Sqrt(2)
        nd(2, 2) = 1 / Math.Sqrt(2)
        nd(0, 3) = 500 * 1 / Math.Sqrt(2)
        nd(1, 3) = 250 * 1 / Math.Sqrt(2)
        nd(2, 3) = 1 / Math.Sqrt(2)
        nd(0, 4) = 500
        nd(1, 4) = 50
        nd(2, 4) = 1

        ReDim Preserve kv2(3)
        kv2(0) = 0
        kv2(1) = 0
        kv2(2) = 1
        kv2(3) = 1




        ReDim Preserve nd2(2, 1)

        nd2(0, 0) = 10
        nd2(1, 0) = 450
        nd2(2, 0) = 1
        nd2(0, 1) = 790
        nd2(1, 1) = 450
        nd2(2, 1) = 1
        Label3.Text = TrackBar4.Value
        Label6.Text = TrackBar1.Value / 100
        Label7.Text = TrackBar2.Value / 100
        Label8.Text = TrackBar3.Value
        Label10.Text = TrackBar5.Value
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '  Dim myGraphics As Graphics
        '  myGraphics = Graphics.FromHwnd(Me.Handle)
        '  myGraphics.Clear(Color.White)
        ' Anode.DeltaZ(1)
        ' Cathode.Crater(0.10000000000000001, Anode)
        '  Anode.DrawCurve()
        ' Cathode.DrawCurve()
        'Cathode.DrawCP()
        ' Cathode.DrawMinDistance()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '  Circle = New Nurbs(nd2, kv2)
        '  Circle.DrawCurve()
        '  Circle.DrawCP()
        '  Circle.DrawHull()
        ' Circle.DrawKnots()
    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim myGraphics As Graphics
        myGraphics = Graphics.FromHwnd(Me.Handle)
        myGraphics.Clear(Me.BackColor)
        Cathode = New Nurbs(nd2, kv2)
        Cathode.Refine(0, 1, TrackBar4.Value)
        Cathode.DrawCurve()
        If CheckBox1.Checked Then
            Cathode.DrawCP()
        End If
        'Cathode.DrawKnots()
    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim myGraphics As Graphics
        For i = 1 To TrackBar5.Value
            Cathode.Crater2(CType(TrackBar1.Value / 100, Double), CType(TrackBar2.Value / 100, Double), TrackBar3.Value)
        Next

        myGraphics = Graphics.FromHwnd(Me.Handle)
        myGraphics.Clear(Me.BackColor)
        If CheckBox1.Checked Then
            Cathode.DrawCP()
        End If
        Cathode.DrawCurve()
        'Cathode.DrawKnots()
    End Sub

    Private Sub Trackbar4_Scroll() Handles TrackBar4.Scroll
        Label3.Text = TrackBar4.Value
    End Sub
    Private Sub Trackbar1_Scroll() Handles TrackBar1.Scroll
        Label6.Text = TrackBar1.Value / 100
    End Sub
    Private Sub Trackbar2_Scroll() Handles TrackBar2.Scroll
        Label7.Text = TrackBar2.Value / 100
    End Sub
    Private Sub Trackbar3_Scroll() Handles TrackBar3.Scroll
        Label8.Text = TrackBar3.Value
    End Sub
    Private Sub Trackbar5_Scroll() Handles TrackBar5.Scroll
        Label10.Text = TrackBar5.Value
    End Sub


End Class