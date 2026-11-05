Imports System.Data.SQLite
Imports System.IO
Imports System.Collections.Generic

Public Module DatabaseInitializer
    Public ConnectionString As String = "Data Source=subaru_parts.db;Version=3;"

    Public Sub InitAndPopulateData(csvFilePath As String)
        If Not File.Exists("subaru_parts.db") Then
            SQLiteConnection.CreateFile("subaru_parts.db")
        End If

        Using conn As New SQLiteConnection(ConnectionString)
            conn.Open()

            Dim createTablesSql As String = "
                CREATE TABLE IF NOT EXISTS Categories (
                    CategoryID INTEGER PRIMARY KEY,
                    CategoryName TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Parts (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    PartNumber TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    DealerCost REAL NOT NULL,
                    MSRP REAL NOT NULL,
                    CategoryID INTEGER NOT NULL,
                    Revision INTEGER DEFAULT 1,
                    IsActive INTEGER DEFAULT 1,
                    EffectiveDate TEXT NOT NULL
                );"

            Using cmd As New SQLiteCommand(createTablesSql, conn)
                cmd.ExecuteNonQuery()
            End Using

            SeedCategories(conn)

            If File.Exists(csvFilePath) Then
                SeedPartsFromCSV(conn, csvFilePath)
            Else
                Console.WriteLine($"Warning: CSV file '{csvFilePath}' not found for seeding.")
            End If
        End Using
    End Sub

    Private Sub SeedCategories(conn As SQLiteConnection)
        Dim checkSql As String = "SELECT COUNT(*) FROM Categories"
        Using checkCmd As New SQLiteCommand(checkSql, conn)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
            If count > 0 Then Return 
        End Using

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

        Using transaction = conn.BeginTransaction()
            Dim insertSql As String = "
                INSERT INTO Categories (CategoryID, CategoryName) VALUES (@catId, @catName)"
            Using cmd As New SQLiteCommand(insertSql, conn)
                cmd.Parameters.Add("@catId", DbType.Int32)
                cmd.Parameters.Add("@catName", DbType.String)

                For Each kvp In categories
                    cmd.Parameters("@catId").Value = kvp.Key
                    cmd.Parameters("@catName").Value = kvp.Value
                    cmd.ExecuteNonQuery()
                Next 
            End Using 
            transaction.Commit()
        End Using

        Console.WriteLine("Successfully seeded 12 categories into database.")
    End Sub

    Private Sub SeedPartsFromCSV(conn As SQLiteConnection, csvFilePath As String)
        Dim checkSql As String = "SELECT COUNT(*) FROM parts"
        Using checkCmd As New SQLiteCommand(checkSql, conn)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
            If count > 0 Then Return
        End Using

        Dim lines As String() = File.ReadAllLines(csvFilePath)
        Dim insertedCount As Integer = 0
        Dim effDateStr As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Using transaction = conn.BeginTransaction()
            Dim insertSql As String = "
                INSERT INTO Parts (
                    PartNumber, Description, DealerCost, MSRP, CategoryID, 
                    Revision, IsActive, EffectiveDate
                )
                VALUES (@partNum, @desc, @cost, @msrp, @catId, 1, 1, @effDate)"

            Using cmd As New SQLiteCommand(insertSql, conn)
                cmd.Parameters.Add("@partNum", DbType.String)
                cmd.Parameters.Add("@desc", DbType.String)
                cmd.Parameters.Add("@cost", DbType.Double)
                cmd.Parameters.Add("@msrp", DbType.Double)
                cmd.Parameters.Add("@catId", DbType.Int32)
                cmd.Parameters.Add("@effDate", DbType.String)

                For i As Integer = 1 to lines.Length - 1
                    Dim line As String = lines(i).Trim()
                    If line.length > 0 Then
                        Dim cols As String() = line.Split(","c)

                        If cols.Length >= 5 Then
                            cmd.Parameters("@partNum").Value = cols(0).Trim()
                            cmd.Parameters("@desc").Value = cols(1).Trim()
                            cmd.Parameters("@cost").Value = CDbl(cols(2).Trim())
                            cmd.Parameters("@msrp").Value = CDbl(cols(3).Trim())
                            cmd.Parameters("@catId").Value = CInt(cols(4).Trim())
                            cmd.Parameters("@effDate").Value = effDateStr

                            cmd.ExecuteNonQuery()
                            insertedCount += 1
                        End If
                    End If
                Next
            End Using
            transaction.Commit()
        End Using

        Console.WriteLine($"Successfully seeded {insertedCount} parts into SQLite database")
    End Sub
End Module
