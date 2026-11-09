Imports System.Collections.Generic
Imports System.IO

Module Program
    Sub Main()
        Console.WriteLine("==========================================")
        Console.WriteLine("        SUBARU PARTS INFORMATION          ")
        Console.WriteLine("==========================================")
        Console.WriteLine()

        Dim csvFilePath As String = "parts.csv"

        If Not File.Exists(csvFilePath) Then
            Console.WriteLine($"Error: data file '{csvFilePath}' was not found.")
            Return 
        End If

        Try 
            Console.WriteLine("Initializing database...")
            DatabaseInitializer.InitAndPopulateData(csvFilePath, forceRecreate:=True)
            Console.WriteLine($"{vbCrLf}Database initialized.")
        Catch e as Exception
            Console.WriteLine($"Error: Failed to initialize database: {e.Message}")
            Return
        End Try

        Dim exitApp As Boolean = False 
        While not exitApp
            Console.WriteLine("==========================================")
            Console.WriteLine("            SUBARU PARTS MENU             ")
            Console.WriteLine("==========================================")   
            Console.WriteLine("1. View All Parts")
            Console.WriteLine("2. View High Profitability Parts (Profit >= $50.00)")
            Console.WriteLine("3. Filter Parts by Category (ID 0-11)")
            Console.WriteLine("4. Search Parts by Description Keyword")
            Console.WriteLine("5. Search Parts by 5 Digit Prefix (Numeric)")
            Console.WriteLine("6. Lookup Active Replacement by Legacy Part Number")
            Console.WriteLine("7. View Full Supersession Chain History")
            Console.WriteLine("8. Exit")
            Console.Write($"{vbCrLf}Select an Option (1-8):{vbCrLf}>")

            Dim choice As String = Console.ReadLine()?.Trim()
            Console.WriteLine()

            Select Case choice
                Case "1"
                    Dim parts = PartQueryService.GetAllPartsWithCategories()
                    DisplayParts(parts, "ALL PARTS CATALOG")
                Case "2"
                    Console.Write(
                        $"Enter minimum profit threshold (default $50.00):{vbCrLf}>"
                    )
                    Dim input As String = Console.ReadLine()?.Trim()
                    Dim minProfit As Double = 50.0

                    If String.IsNullOrEmpty(input) OrElse _
                        Double.TryParse(input, minProfit) Then 

                        Dim parts = 
                            PartQueryService.GetHighProfitabilityParts(minProfit)
                        
                        DisplayParts(
                            parts, $"HIGH PROFITABILITY PARTS (>= ${minProfit:F2})"
                        )
                    Else
                        Console.WriteLine("Invalid number entered.")
                    End If
                Case "3"
                    Console.Write($"Enter Category ID (0-11):{vbCrLf}>")
                    Dim catInput As String = Console.ReadLine()?.Trim()
                    Dim catId As Integer
                    If Integer.TryParse(catInput, catId) AndAlso _
                        catId >= 0 AndAlso _
                        catId <= 11 Then
                        
                        Dim parts = PartQueryService.GetPartsByCategory(catId)
                        Dim categoryName As String =
                            PartQueryService.GetCategoryName(catId)

                        DisplayParts(parts, $"PARTS FOR '{categoryName}'")
                    Else
                        Console.WriteLine(
                            "Invalid Category ID. Must be an integer between 0 and 11."
                        )
                    End If
                Case "4"
                    Console.Write($"Enter keyword to search description " & 
                                  $"(e.g. filter, brake):{vbCrLf}>"
                    )
                    Dim keyword As String = Console.ReadLine()?.Trim()
                    If Not String.IsNullOrEmpty(keyword) Then
                        Dim parts = PartQueryService.SearchPartsByKeyword(keyword)
                        DisplayParts(parts, $"SEARCH RESULTS FOR KEYWORD '{keyword}'")
                    Else
                        Console.WriteLine("Keyword cannot be empty.")
                    End If
                Case "5"
                    Console.Write(
                        $"Enter 5 digit numeric prefix (e.g. 16546):{vbCrLf}>"
                    )
                    Dim prefix As String = Console.ReadLine()?.Trim()
                    If Not String.IsNullOrEmpty(prefix) Then
                        Dim parts = PartQueryService.GetPartsByNumericPrefix(prefix)
                        DisplayParts(parts, $"PARTS MATCHING PREFIX '{prefix}'")
                    Else 
                        Console.WriteLine("Prefix cannot be empty.")
                    End If
                Case "6"
                    Console.Write(
                        $"Enter legacy part number to find active replacement " &
                        $":{vbCrLf}>"
                    )
                    Dim legacyPartNum As String = Console.ReadLine()?.Trim()
                    If Not String.IsNullOrEmpty(legacyPartNum) Then 
                        Dim activePart As PartRecord = 
                            PartQueryService.GetActiveReplacement(legacyPartNum)
                        Console.WriteLine($" === SUPERSESSION REDIRECT RESULTS ===")
                        If activePart IsNot Nothing Then 
                            Console.WriteLine($"Original Part " &
                                              $"Searched: {legacyPartNum.ToUpper()}")
                            Console.WriteLine($"Active Replacement Found:") 
                            Console.WriteLine(activePart.ToString())
                        Else 
                            Console.WriteLine($"No active replacement found for " &
                                              $"part '{legacyPartNum}'")
                        End If
                    Else 
                        Console.WriteLine("Part number cannot be empty.")
                    End If
                Case "7"
                    Console.Write(
                        $"Enter a part number to view full revision history " &
                        $":{vbCrLf}>"
                    )
                    Dim inputPartNum As String = Console.ReadLine()?.Trim()
                    If Not String.IsNullOrEmpty(inputPartNum) Then 
                        Dim history = 
                            PartQueryService.GetSupersessionHistory(inputPartNum)
                        DisplayParts(
                            history,
                            $"FULL REVISION HISTORY FOR '{inputPartNum.ToUpper()}'"
                        )
                    Else
                        Console.WriteLine("Part number cannot be empty.")
                    End If
                Case "8"
                    exitApp = True 
                    Console.WriteLine("Exiting Subaru Parts Tool.")
                Case Else 
                    Console.WriteLine("Invalid choice. Please select 1 through 8.")
            End Select

            If Not exitApp Then
                Console.WriteLine($"{vbCrLf}Press any key to return to menu...")
                Console.ReadKey()
                Console.Clear()
            End If
        End While
    End Sub

    Private Sub DisplayParts(parts As List(Of PartRecord), headerTitle As String)
        Console.WriteLine($"=== {headerTitle} ({parts.Count} items found) ===")
        If parts.Count = 0 Then
            Console.WriteLine("No matching records found.")
            Return
        End If

        For Each part In parts
            Console.WriteLine(part.ToString())
        Next
    End Sub
End Module
