
Imports Windows.Web.Syndication

Public Class RSShelperClass
    Private Async Sub load(list As ItemsControl, uri As Uri)
        Dim client As New SyndicationClient()
        Try
            Dim feed As SyndicationFeed = Await client.RetrieveFeedAsync(uri)
            If feed IsNot Nothing Then
                For Each item As SyndicationItem In feed.Items
                    list.Items.Add(item)
                Next


                Dim Settings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
                If Settings.Values.ContainsKey("LastNews") = False Then
                    Settings.Values.Add("LastNews", feed.Items(0).Title.Text)
                Else
                    Settings.Values("LastNews") = feed.Items(0).Title.Text
                End If
            End If

        Catch ex As Exception

        End Try

    End Sub

    Public Sub Go(ByRef list As ItemsControl, value As String)

        Try
            load(list, New Uri(value))

        Catch
        End Try
        list.Focus(FocusState.Keyboard)

    End Sub
End Class

