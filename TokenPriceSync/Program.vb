Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading

Module Program
    Public myDB As New MySqlDatabase
    Sub Main()

        Console.WriteLine("Connecting to Database:")
        If myDB.EstablishConnection() Then
            Console.WriteLine("Connected to Database.")
            Console.WriteLine("Import Token Price Started:")

            Dim _timer As Timer
            'trigger the actions every 5minutes
            _timer = New Timer(AddressOf TimerCallback, Nothing, 0, 300000)
            Console.WriteLine("Import Token Price Completed.")
            Console.ReadLine()
            myDB.Close()
        Else
            Console.WriteLine("Please Check your Connection String in App.")
        End If
    End Sub

    Private Sub TimerCallback(obj As Object)
        Console.WriteLine("Date and Time with Milliseconds: {0}",
                    Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"))
        UpdateTokenPrice()
    End Sub

    Private Class TokenPrice
        Public Property USD As Decimal
    End Class

    ''' <summary>
    ''' update all the token type in database and synchronize with cryptocompare
    ''' </summary>
    Private Async Sub UpdateTokenPrice()
        Dim tokenDataTable As New DataTable
        tokenDataTable = myDB.GetDataTable("SELECT ID, Symbol FROM Token")
        For Each records As DataRow In tokenDataTable.Rows
            Dim tokenPrice As TokenPrice = Await DownloadTokenAsync(records.Item("symbol"))
            myDB.ExecuteQuery($"update token set price = {tokenPrice.USD} where symbol = '{records.Item("symbol")}' and id = {records.Item("ID")}")
        Next
    End Sub

    ''' <summary>
    ''' Download token information. 
    ''' </summary>
    ''' <param name="symbol">token type</param>
    ''' <returns></returns>
    Private Async Function DownloadTokenAsync(symbol As String) As Task(Of TokenPrice)
        Dim page As String = $"https://min-api.cryptocompare.com/data/price?fsym={symbol}&tsyms=USD"
        Dim result As String
        ' Use HttpClient in Using-statement.
        ' ... Use GetAsync to get the page data.
        Using client As HttpClient = New HttpClient()
            Using response As HttpResponseMessage = Await client.GetAsync(page)
                Using content As HttpContent = response.Content
                    ' Get contents of page as a String.
                    result = Await content.ReadAsStringAsync()
                    ' If data exists, print a substring.
                End Using
            End Using
        End Using
        'if there is invalid symbol or other errors returning, TokenPrice amount will default to 0
        Return JsonConvert.DeserializeObject(Of TokenPrice)(result)
    End Function


End Module
