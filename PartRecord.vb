Public Class PartRecord
    Public Property PartNumber As String
    Public Property Description As String
    Public Property DealerCost As Double 
    Public Property MSRP As Double
    Public Property CategoryID As Integer

    Public Property Revision As Integer 
    Public Property EffectiveDate As DateTime
    Public Property IsActive As Boolean

    Public Sub New(partNum As String, 
                   desc As String, 
                   cost As Double, 
                   msrpVal As Double, 
                   catId As Integer,
                   Optional rev As Integer = 1,
                   Optional active As Boolean = True)
        Me.PartNumber = partNum 
        Me.Description = desc
        Me.DealerCost = cost 
        Me.MSRP = msrpVal
        Me.CategoryID = catId

        Me.Revision = rev
        Me.EffectiveDate = DateTime.Now
        Me.IsActive = active
    End Sub

    Public ReadOnly Property ProjectedProfitMargin As Double
        Get
            Return MSRP - DealerCost
        End Get
    End Property

    Public Overrides Function ToString() As String 
        Dim status As String = If(IsActive, "ACTIVE", "HISTORICAL")
        Return $"{PartNumber,-15} | {Description,-55} | Rev {Revision,2} | {status,-10} | " &
               $"Cost: ${DealerCost,7:F2} | MSRP: ${MSRP,7:F2} | " &
               $"Profit: ${ProjectedProfitMargin,7:F2} | Cat ID: {CategoryID,2}"
    End Function
End Class
