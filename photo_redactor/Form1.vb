Imports System.Drawing.Drawing2D

Public Class Form1
    Dim file_path As String
    Dim bm As Bitmap
    Dim color_new As Color
    Dim kd As Boolean = False
    Dim reg As Boolean = False
    Dim x0, y0, x, y As Integer
    Dim stp_x, stp_y As Integer
    Dim r, g, b As Integer
    Dim l As Integer = 0
    Dim obl_x0, obl_y0, obl_x, obl_y As Integer
    Dim new_size As Size

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ColorDialog1.ShowDialog()
        color_new = ColorDialog1.Color
        Button2.BackColor = color_new
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        SaveFileDialog1.Filter = "BMP|*.bmp|JPG|*.jpg|JPEG|*.jpeg"
        If Not IsNothing(bm) Then
            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                If IO.File.Exists(SaveFileDialog1.FileName) Then
                    IO.File.Delete(SaveFileDialog1.FileName)
                End If
                PictureBox1.Image.Save(SaveFileDialog1.FileName)
                bm.Dispose()
            End If
        End If
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button2.BackColor = Color.Black
        color_new = Color.Black
        For i = 50 To 1 Step -1
            DomainUpDown1.Items.Add(i)
        Next
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then
            file_path = OpenFileDialog1.FileName
            bm = Image.FromFile(file_path)
            If bm.Height > PictureBox1.Height Then
                new_size = New Size(bm.Width, PictureBox1.Height)
                bm = resize_image(bm, new_size)
            End If
            If bm.Width > PictureBox1.Width Then
                new_size = New Size(PictureBox1.Width, bm.Height)
                bm = resize_image(bm, new_size)
            End If
            PictureBox1.Image = bm
        End If

    End Sub


    Function resize_image(image As Image, newSize As Size) As Image
        Dim result As New Bitmap(newSize.Width, newSize.Height)
        Using g As Graphics = Graphics.FromImage(result)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.DrawImage(image, New Rectangle(New Point(), newSize), New Rectangle(New Point(), image.Size), GraphicsUnit.Pixel)
        End Using
        Return result
    End Function


    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        kd = False
        reg = False
        If RadioButton1.Checked Then
            revers()
        ElseIf RadioButton3.Checked Then
            blur()
        End If

    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        kd = True
        If kd And (Not IsNothing(bm)) Then
            If RadioButton2.Checked Then
                paintin(e.X, e.Y, CInt(DomainUpDown1.Text))
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If kd And (Not IsNothing(bm)) Then
            If RadioButton1.Checked Or RadioButton3.Checked Then
                vid(e.X, e.Y)
            ElseIf RadioButton2.Checked Then
                paintin(e.X, e.Y, CInt(DomainUpDown1.Text))
            End If
        End If

    End Sub


    Sub vid(ex As Integer, ey As Integer)
        If reg Then
            If ex < bm.Width And ex > 1 Then
                x = ex
            End If
            If ey < bm.Height And ey > 1 Then
                y = ey
            End If
            If x0 < x Then
                stp_x = 1
            Else
                stp_x = -1
            End If
            If y0 < y Then
                stp_y = 1
            Else
                stp_y = -1
            End If
            directly_vid()
        Else
            If ex < bm.Width And ex > 1 Then
                x0 = ex
            End If
            If ey < bm.Height And ey > 1 Then
                y0 = ey
            End If
            reg = True
        End If

    End Sub

    Sub revers()
        For i = x0 To x Step stp_x
            For j = y0 To y Step stp_y
                r = bm.GetPixel(i, j).R
                g = bm.GetPixel(i, j).G
                b = bm.GetPixel(i, j).B
                bm.SetPixel(i, j, Color.FromArgb(255 - r, 255 - g, 255 - b))
                PictureBox1.Image = bm
            Next
        Next
    End Sub

    Sub directly_vid()
        For i = x0 To x Step stp_x
            Dim graph As Graphics
            Dim pen2 As New Pen(Color.Black, 1)
            graph = PictureBox1.CreateGraphics
            For j = y0 To y Step stp_y
                If x0 > x Then
                    obl_x0 = x
                    obl_x = x0
                Else
                    obl_x0 = x0
                    obl_x = x
                End If
                If y0 > y Then
                    obl_y0 = y
                    obl_y = y0
                Else
                    obl_y0 = y0
                    obl_y = y
                End If
            Next
            graph.DrawRectangle(pen2, obl_x0, obl_y0, obl_x - obl_x0, obl_y - obl_y0)
            PictureBox1.Image = bm
            graph.Dispose()
            pen2.Dispose()
        Next
        PictureBox1.Refresh()

    End Sub

    Sub paintin(ex As Integer, ey As Integer, rd As Integer)
        If ex < bm.Width And ey < bm.Height And ex > 0 And ey > 0 Then
            For xx = ex - rd \ 2 To ex + rd \ 2
                For yy = ey - rd \ 2 To ey + rd \ 2
                    If xx < bm.Width And yy < bm.Height And xx > 0 And yy > 0 Then
                        bm.SetPixel(xx, yy, color_new)
                    End If
                Next
            Next
            PictureBox1.Image = bm
        End If
    End Sub

    Sub blur()
        PictureBox1.Refresh()
        PictureBox1.Image = bm
        r = 0
        g = 0
        b = 0
        For i = x0 To x Step stp_x
            r = 0
            g = 0
            b = 0
            l = 0
            For j = y0 To y Step stp_y
                r += bm.GetPixel(i, j).R
                g += bm.GetPixel(i, j).G
                b += bm.GetPixel(i, j).B
                l += 1
                If l Mod 3 = 0 And l > 0 Then
                    If (stp_x = 1 Or stp_x = -1) And stp_y = 1 Then
                        bm.SetPixel(i, j, Color.FromArgb(r \ 3, g \ 3, b \ 3))
                        bm.SetPixel(i, j - 1, Color.FromArgb(r \ 3, g \ 3, b \ 3))
                        bm.SetPixel(i, j - 2, Color.FromArgb(r \ 3, g \ 3, b \ 3))
                    Else
                        bm.SetPixel(i, j, Color.FromArgb(r \ 3, g \ 3, b \ 3))
                        bm.SetPixel(i, j + 1, Color.FromArgb(r \ 3, g \ 3, b \ 3))
                        bm.SetPixel(i, j + 2, Color.FromArgb(r \ 3, g \ 3, b \ 3))
                    End If
                    r = 0
                    g = 0
                    b = 0
                End If
            Next
        Next
        PictureBox1.Image = bm
    End Sub


End Class
