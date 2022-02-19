Imports MySql.Data.MySqlClient
Imports System.Configuration

Public Class MySqlDatabase
    Private conn As New MySqlConnection
    ''' <summary>
    ''' Ensure the table able to reach
    ''' </summary>
    Public Function EstablishConnection() As Boolean
        Try
            If conn.State = ConnectionState.Closed Then
                conn.ConnectionString = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
                conn.Open()
            End If
            Return True
        Catch ex As MySqlException
            Console.WriteLine($"Connection Failed: {ex.Message}")
            Return False
        End Try
    End Function

    Public Sub Close()
        conn.Close()
    End Sub


    ''' <summary>
    ''' Execute query for Insert, Update and Delete
    ''' </summary>
    ''' <param name="sql"></param>
    Public Sub ExecuteQuery(sql As String)

        Dim result As String
        Me.EstablishConnection()
        Using cmd As New MySqlCommand
            cmd.Connection = conn
            cmd.CommandText = sql
            'EXECUTE THE DATA
            result = cmd.ExecuteNonQuery
            'CHECKING IF THE DATA HAS EXECUTED OR NOT AND THEN THE POP UP MESSAGE WILL APPEAR
            If result = 0 Then
                Console.WriteLine("Failed")
            End If
        End Using
    End Sub

    Public Function GetDataTable(ByVal sql As String) As DataTable
        Try
            Me.EstablishConnection()
            Dim dt As New DataTable
            Using cmd As New MySqlCommand
                cmd.Connection = conn
                cmd.CommandText = sql
                Using da As New MySqlDataAdapter
                    da.SelectCommand = cmd
                    da.Fill(dt)
                End Using
            End Using
            Return dt
        Catch ex As Exception
            Return New DataTable()
            Console.WriteLine(ex.Message)
        End Try
    End Function

End Class
