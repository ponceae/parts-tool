Imports System.IO
Imports System.Collections.Generic

Module Program
    Sub Main()
        Dim categories As New Dictionary(Of Integer, String) From {
            {0, "Engine"},
            {1, "Transmission"},
            {2, "Suspension, Axle and Brake"},
            {3, "Steering System and Cable"},
            {4, "Engine Mount, Fuel, Exhaust and Engine Cooling"},
            {5, "Body Parts"},
            {6, "Door, Seat, Windshield, and Instrument Panel"},
            {7, "Heater and Air Condition"},
            {8, "Electrical Parts"},
            {9, "Sealing, Accessories and Trim"},
            {10, "Subaru of America Genuine Accessories"},
            {11, "Paint & Chemicals"}
        }

        Dim fp As String = "parts.csv"

        If Not File.Exists(fp) Then
            Console.WriteLine($"Error: File '{fp}' was not found.")
            Return
        End If

        Dim partsList As New List(Of PartRecord)()
        Dim lines As String() = File.ReadAllLines(fp)

        For i As Integer = 1 To lines.length - 1
            Dim line As String = lines(i).Trim()

            If line.length > 0 Then 
                Dim cols As String() = line.Split(","c)

                If cols.length >= 5 Then
                    Dim partNum As String = cols(0)
                    Dim desc As String = cols(1).Trim()
                    Dim cost As Double = CDbl(cols(2).trim())
                    Dim msrpVal As Double = CDbl(cols(3).Trim())
                    Dim catId As Integer = CInt(cols(4).Trim())

                    Dim record As New PartRecord(partNum, desc, cost, msrpVal, catId)
                    partsList.Add(record)
                End If
            End If
        Next

        Console.WriteLine($"Successfully loaded {partsList.Count} parts into memory.")
        Console.WriteLine(New String("-"c,100))

        For Each part as PartRecord in partsList
            Dim catName As String = If(categories.ContainsKey(part.CategoryID), categories(part.CategoryID), "Unknown")
            Console.WriteLine($"{part.ToString()} | {catName,-45}")
        Next
    End Sub
End Module
