Imports System.Drawing.Drawing2D
Imports DotNumerics.Optimization
Public Class Nurbs

    'This class provides tools to create, visualize and modify nurbs curves

    'Attributes
    'Nurbsdata is a two-dimension array containing control point coordinates 
    'and associated weight in this format : (w.X,w.Y,w)

    Dim nurbsdata As Double(,)
    Dim a As Nurbs
    Dim numcp As Integer
    Dim knotsvector As New List(Of Double)
    Dim test As Array
    Dim numknots As Integer
    Dim myGraphics As Graphics
    Dim myPen As New Pen(Color.Blue)
    Private Delegate Function WarpingFunction(ByVal u As Double)

    'Constructeur
    Public Sub New(ByVal cp As Double(,), ByVal knots As Double())
        Dim i As Integer = 0
        nurbsdata = cp
        numcp = cp.GetLength(1)
        For i = 0 To knots.GetLength(0) - 1
            knotsvector.Add(knots(i))
        Next

        numknots = knots.GetLength(0)
    End Sub


    'Fonctions de courbes
    Public Sub InsertKnot(ByVal u As Double)
        Dim k As Integer
        Dim alpha As Double()
        Dim newCP As Double(,)
        Dim p As Integer = numknots - numcp - 1
        Dim i, n As Integer
        'Let's find where the knot will be inserted
        k = FindSpan(u)


        'Calculating the alpha coefficients
        For i = k - p + 1 To k
            ReDim Preserve alpha(i)
            alpha(i) = (u - knotsvector(i)) / (knotsvector(i + p) - knotsvector(i))
        Next
        'Building the new knotsvector
        knotsvector.Insert(k + 1, u)
        'ReDim Preserve newKnotsVector(numknots)
        'For i = 0 To numknots
        '    If i <= k Then
        '        newKnotsVector(i) = knotsvector(i)
        '    ElseIf i = k + 1 Then
        '        newKnotsVector(i) = u
        '    Else
        '        newKnotsVector(i) = knotsvector(i - 1)
        '    End If
        'Next
        'Calculating the new control points
        ReDim Preserve newCP(2, numcp)
        For i = 0 To numcp
            If i <= k - p Then
                For n = 0 To 2
                    newCP(n, i) = nurbsdata(n, i)
                Next
            ElseIf i > k Then
                For n = 0 To 2
                    newCP(n, i) = nurbsdata(n, i - 1)
                Next
            Else
                For n = 0 To 2
                    newCP(n, i) = alpha(i) * nurbsdata(n, i) + (1 - alpha(i)) * nurbsdata(n, i - 1)
                Next
            End If
        Next
        ReDim Preserve nurbsdata(2, numcp)
        'ReDim Preserve knotsvector(numknots)
        nurbsdata = newCP
        numcp = nurbsdata.GetLength(1)
        'knotsvector = newKnotsVector
        numknots = numknots + 1
    End Sub

    Public Function BasisFunctions(ByVal u As Double)

        Dim N As Double()

        Dim p As Integer = numknots - numcp - 1

        Dim temp As Double()

        Dim d, e, span As Double

        Dim Sum As Double

        Dim j, r, k, i As Integer

        j = 1

        r = 0

        span = FindSpan(u)
        'First order calculations

        For k = span - p To span + p

            If u >= knotsvector(k) And u < knotsvector(k + 1) Then

                ReDim Preserve temp(k)

                temp(k) = 1
            ElseIf u = 1 And k = span Then

                ReDim Preserve temp(k)
                temp(k) = 1

            Else

                ReDim Preserve temp(k)

                temp(k) = 0

            End If

        Next


        'Other orders calculations 


        For k = 1 To p

            For i = span - p To span + p - k

                If temp(i) <> 0 Then
                    d = (u - knotsvector(i)) * temp(i) / (knotsvector(i + k) - knotsvector(i))

                Else

                    d = 0

                End If


                If temp(i + 1) <> 0 Then

                    e = (knotsvector(i + 1 + k) - u) * temp(i + 1) / (knotsvector(i + k + 1) - knotsvector(i + 1))

                Else

                    e = 0

                End If

                temp(i) = d + e


            Next


        Next

        For i = span - p To span

            Sum = Sum + temp(i) * nurbsdata(2, i)

        Next

        For i = span - p To span

            If Sum <> 0 Then

                ReDim Preserve N(i)

                N(i) = temp(i) * nurbsdata(2, i) / Sum

            Else

                ReDim Preserve N(i)

                N(i) = 0

            End If

        Next



        Return N

    End Function

    Private Function FindSpan(ByVal u As Double)

        Dim n As Integer = numcp - 1
        Dim p As Integer = numknots - numcp - 1
        If u >= knotsvector(n + 1) Then

            Return n

            Exit Function

        End If


        If u < knotsvector(p + 1) Then

            Return p

            Exit Function

        End If



        Dim low, high, mid As Integer

        low = p

        high = n + 1

        mid = (low + high) / 2

        While u < knotsvector(mid) Or u >= knotsvector(mid + 1)

            If u < knotsvector(mid) Then

                high = mid

            Else

                low = mid

            End If

            mid = (low + high) / 2

        End While

        Return mid


    End Function

    Public Function CurvePoint(ByVal u As Double)

        Dim span, j, i As Integer

        Dim p As Integer = numknots - numcp - 1

        Dim Functions As Double()

        Dim Cw As Double()

        ReDim Preserve Cw(1)

        span = FindSpan(u)

        Functions = BasisFunctions(u)

        For j = span - p To span

            For i = 0 To 1

                Cw(i) = Cw(i) + Functions(j) * nurbsdata(i, j) / nurbsdata(2, j)

            Next


        Next
        Return Cw

    End Function

    'Fonctions de déformation
    Public Sub Refine(ByVal s As Double, ByVal e As Double, ByVal n As Integer)
        Dim i, k As Integer
        Dim ks, ke, w As Integer
        Dim p As Integer = numknots - numcp - 1
        Dim w2 As Double = 0
        'Let's create knots at the start and the end of the region
        ks = FindSpan(s) + 1
        ke = FindSpan(e) + 2
        InsertKnot(s)
        InsertKnot(e)

        'Let's recursively create n knots inside the segment
        For i = 0 To n - 1
            w = ks

            'Searching for the widest span
            For k = ks To ke - 1
                If knotsvector(k + 1) - knotsvector(k) > w2 Then

                    w = k
                    w2 = knotsvector(k + 1) - knotsvector(k)
                End If
            Next
            InsertKnot(knotsvector(w) + (knotsvector(w + 1) - knotsvector(w)) / 2)
            ke = ke + 1
            w2 = 0
        Next



    End Sub

    Public Function CraterDirection(ByVal a As Nurbs, ByVal u As Double, ByVal v As Double)
        Dim Direction As Double()
        Dim Norm As Double
        ReDim Preserve Direction(1)
        Direction(0) = CurvePoint(u)(0) - a.CurvePoint(v)(0)
        Direction(1) = CurvePoint(u)(1) - a.CurvePoint(v)(1)
        Norm = Math.Sqrt(Math.Pow(Direction(0), 2) + Math.Pow(Direction(1), 2))
        Direction(0) = Direction(0) / Norm
        Direction(1) = Direction(1) / Norm
        Return Direction
    End Function

    Public Function CraterFunction(ByVal u As Double)
        Dim ndwarp As Double(,)
        Dim kvwarp As Double() = {0, 0, 0, 0, 0.25, 0.5, 0.75, 1, 1, 1, 1}
        Dim Output As Double
        ReDim Preserve ndwarp(2, 6)
        ndwarp(2, 0) = 1
        ndwarp(2, 1) = 1
        ndwarp(2, 2) = 1
        ndwarp(2, 3) = 30
        ndwarp(2, 4) = 1
        ndwarp(2, 5) = 1
        ndwarp(2, 6) = 1
        Dim Warp As New Nurbs(ndwarp, kvwarp)
        'Warp.DrawBasis()
        Output = Warp.BasisFunctions(u)(3)
        Return Output
    End Function

    Public Sub Crater(ByVal interval As Double, ByVal a As Nurbs)
        Dim parameter As Double()
        Dim chordal As Double
        Dim tempchordal As Double
        parameter = MinDistance()
        Dim min As Double = parameter(0) - interval / 2
        Dim max As Double = parameter(0) + interval / 2
        Dim spanspan As Integer = FindSpan(max) - FindSpan(min)
        Dim Direction As Double()
        Direction = CraterDirection(a, parameter(0), parameter(1))
        Dim i As Integer
        For i = FindSpan(min) To FindSpan(max)
            chordal = chordal + Math.Sqrt(Math.Pow(nurbsdata(0, i), 2) + Math.Pow(nurbsdata(1, i), 2))
        Next
        Dim k As Integer
        For k = FindSpan(min) To FindSpan(max)
            tempchordal = tempchordal + Math.Sqrt(Math.Pow(nurbsdata(0, k), 2) + Math.Pow(nurbsdata(1, k), 2))
            nurbsdata(0, k) = nurbsdata(0, k) + CraterFunction(tempchordal / chordal) * Direction(0)
            nurbsdata(1, k) = nurbsdata(1, k) + CraterFunction(tempchordal / chordal) * Direction(1)
        Next

    End Sub

    Public Sub Crater2(ByVal interval As Double, ByVal u As Double, ByVal x As Integer)
        Dim chordal As Double
        Dim tempchordal As Double
        'parameter = MinDistance()
        Dim min As Double = u - interval / 2
        Dim max As Double = u + interval / 2
        Dim Direction As Double()
        Direction = {0, 1}
        Dim i As Integer
        For i = FindSpan(min) To FindSpan(max) - 1
            chordal = chordal + Math.Sqrt(Math.Pow(nurbsdata(0, i), 2) + Math.Pow(nurbsdata(1, i), 2))
        Next
        Dim k As Integer
        For k = FindSpan(min) To FindSpan(max) - 1
            tempchordal = tempchordal + Math.Sqrt(Math.Pow(nurbsdata(0, k), 2) + Math.Pow(nurbsdata(1, k), 2))
            nurbsdata(0, k) = nurbsdata(0, k) + x * CraterFunction(tempchordal / chordal) * Direction(0)
            nurbsdata(1, k) = nurbsdata(1, k) + x * CraterFunction(tempchordal / chordal) * Direction(1)
        Next

    End Sub


    'Fonctions de distance
    Public Function Distance2(ByVal x As Double())
        Return Math.Pow(CurvePoint(x(0))(0) - a.CurvePoint(x(1))(0), 2) + Math.Pow(CurvePoint(x(0))(1) - a.CurvePoint(x(1))(1), 2)
    End Function


    'Public Function MinDistance(ByVal b As Nurbs)

    '    Dim epsilon As Double
    '    Dim u, v As Double
    '    epsilon = 0.01

    '    Dim min As Double()

    '    ReDim Preserve min(1)

    '    min(0) = 0

    '    min(1) = 0

    '    For u = 0 To 1 Step epsilon

    '        For v = 0 To 1 Step epsilon

    '            If Distance(b, u, v) < Distance(b, min(0), min(1)) Then

    '                min(0) = u

    '                min(1) = v

    '            End If

    '        Next

    '    Next

    '    Return min

    'End Function

    'Public Function MinDistance2(ByVal a As Nurbs)
    '    Dim min As Double()
    '    Dim delta As Double
    '    Dim center, north, south, east, west As Double
    '    Dim x, tempx As Double
    '    Dim y, tempy As Double
    '    Dim small As Double
    '    delta = 0.5
    '    x = 0.5
    '    tempx = 0
    '    y = 0.5
    '    tempy = 0
    '    While delta > 0.001
    '        center = Distance(a, x, y)
    '        north = Distance(a, x, y + delta)
    '        south = Distance(a, x, y - delta)
    '        east = Distance(a, x - delta, y)
    '        west = Distance(a, x + delta, y)
    '        small = center
    '        If north < small Then
    '            tempx = x
    '            tempy = y + delta
    '        ElseIf south < small Then
    '            tempx = x
    '            tempy = y - delta
    '        ElseIf east < small Then
    '            tempx = x - delta
    '            tempy = y
    '        ElseIf west < small Then
    '            tempx = x + delta
    '            tempy = y
    '        Else
    '            delta = delta / 2
    '            tempx = x
    '            tempy = y
    '        End If
    '        x = tempx
    '        y = tempy
    '    End While
    '    ReDim Preserve min(1)
    '    min(0) = x
    '    min(1) = y
    '    Return min
    'End Function

    Public Function GradDistance(ByVal x As Double())
        Dim Gradient As Double() = {0, 0}
        Dim h As Double
        h = 0.001
        Gradient(0) = (Distance2({x(0) + h, x(1)}) - Distance2(x)) / h
        Gradient(1) = (Distance2({x(0), x(1) + h}) - Distance2(x)) / h
        Return Gradient
    End Function

    Public Function MinDistance()
        Dim BFGS As New L_BFGS_B
        Dim Gradient As New OptMultivariateGradient(AddressOf GradDistance)
        Dim Distance As New OptMultivariateFunction(AddressOf Distance2)
        Dim InitialGuess As Double()
        ReDim Preserve InitialGuess(1)
        InitialGuess(0) = 0
        InitialGuess(1) = 1
        Dim min As Double()
        min = BFGS.ComputeMin(Distance, Gradient, InitialGuess)
        Return min

    End Function

    'Fonctions de dessin

    Public Sub DrawCP()
        myGraphics = Graphics.FromHwnd(Form1.Handle)
        myGraphics.SmoothingMode = SmoothingMode.AntiAlias


        Dim i As Integer

        For i = 0 To numcp - 1

            myGraphics.DrawEllipse(pen:=New Pen(Color.Blue), rect:=New Rectangle(x:=nurbsdata(0, i) / nurbsdata(2, i) - 2, y:=nurbsdata(1, i) / nurbsdata(2, i) - 2, width:=4, height:=4))

        Next

    End Sub


    Public Sub DrawKnots()
        Dim myPen As New Pen(Color.Green)
        Dim temp As Double()

        ReDim Preserve temp(1)
        myGraphics = Graphics.FromHwnd(Form1.Handle)
        myGraphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim i As Integer

        For i = 0 To numknots - 1
            temp = CurvePoint(knotsvector(i))
            myGraphics.DrawEllipse(pen:=myPen, rect:=New Rectangle(x:=temp(0) - 2, y:=temp(1) - 2, width:=4, height:=4))

        Next

    End Sub


    Public Sub DrawHull()


        myGraphics = Graphics.FromHwnd(Form1.Handle)
        myGraphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim myPen As New Pen(Color.Blue)

        Dim i As Integer

        For i = 0 To numcp - 2

            myGraphics.DrawLine(pen:=myPen, x1:=CType(nurbsdata(0, i) / nurbsdata(2, i), Integer), y1:=CType(nurbsdata(1, i) / nurbsdata(2, i), Integer), x2:=CType(nurbsdata(0, i + 1) / nurbsdata(2, i + 1), Integer), y2:=CType(nurbsdata(1, i + 1) / nurbsdata(2, i + 1), Integer))

        Next

    End Sub


    Public Sub DrawBasis()

        Dim u As Double

        Dim d As Double
        Dim span, k As Integer

        Dim n As Integer = 1000

        d = 1 / n


        Dim myPen As New Pen(Color.Red)

        Dim myPen2 As New Pen(Color.Blue)

        Dim myPen3 As New Pen(Color.Purple)

        Dim myPen4 As New Pen(Color.Black)

        myGraphics = Graphics.FromHwnd(Form1.Handle)
        myGraphics.SmoothingMode = SmoothingMode.AntiAlias
        While u <= 1

            If u + d > 1 Then

                Exit Sub

            End If

            span = FindSpan(u)

            myGraphics.DrawLine(pen:=myPen, x1:=CType(u * 500, Single), y1:=CType(500 * BasisFunctions(u)(3), Single), x2:=CType((u + d) * 500, Single), y2:=CType(500 * BasisFunctions(u + d)(3), Single))




            u = u + d

        End While

    End Sub


    Public Sub DrawCurve()
        Dim temp As Double()
        Dim u As Double
        Dim n As Integer = 1000
        Dim d As Double = 1 / n
        Dim myPen As New Pen(Color.Red)
        ReDim Preserve temp(1)

        myGraphics = Graphics.FromHwnd(Form1.Handle)
        myGraphics.SmoothingMode = SmoothingMode.AntiAlias
        u = 0
        While u <= 1

            If u + d > 1 Then

                Exit Sub

            End If
            temp = CurvePoint(u)
            myGraphics.DrawLine(pen:=myPen, x1:=CType(temp(0), Single), y1:=CType(temp(1), Single), x2:=CType(CurvePoint(u + d)(0), Single), y2:=CType(CurvePoint(u + d)(1), Single))

            u = u + d

        End While


    End Sub


    Public Sub DrawMinDistance()


        myGraphics = Graphics.FromHwnd(Form1.Handle)

        myGraphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim myPen As New Pen(Color.Green)

        Dim i As Integer

        Dim min As Double()

        min = MinDistance()



        myGraphics.DrawLine(pen:=myPen, x1:=CType(CurvePoint(min(0))(0), Single), y1:=CType(CurvePoint(min(0))(1), Single), x2:=CType(a.CurvePoint(min(1))(0), Single), y2:=CType(a.CurvePoint(min(1))(1), Single))



    End Sub

    Public Sub DeltaZ(ByVal z As Double)
        For i = 0 To nurbsdata.GetLength(1) - 1
            nurbsdata(1, i) = nurbsdata(1, i) + z * nurbsdata(2, i)
        Next

    End Sub






    'Proprietes

    Public Property Data()

        Get

            Return nurbsdata

        End Get

        Set(ByVal value)

            nurbsdata = value

            numcp = value.getlength(1)

        End Set

    End Property

    Public Property PNumCP()
        Get
            Return nurbsdata.GetLength(1)
        End Get
        Set(ByVal value)
            numcp = value
        End Set
    End Property

    Public Property PNumKnots()
        Get
            Return numknots
        End Get
        Set(ByVal value)
            numknots = value
        End Set
    End Property

    Public Property PNurbsDistance()
        Get
            Return a
        End Get
        Set(ByVal value)
            a = value
        End Set
    End Property








End Class
