
Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Runtime.Serialization.Json
Imports System.Text

Namespace KCommon
    Module KCommon
        Public UnsuportedSpeechLetter As Char() = "œÃÕŒÂ⁄€›ﬁÀ’÷‘”Ì»·« ‰„ﬂÿŸ“Ê…ÏÏ·«—ƒ¡∆".ToCharArray
    End Module
    Public NotInheritable Class IsolatedStorageHelper
        Private Sub New()
        End Sub
        Public Shared Function GetObject(Of T)(ByVal key As String) As T
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

            If localSettings.Values.ContainsKey(key) Then
                Dim serializedObject As String = localSettings.Values(key).ToString()
                Return Deserialize(Of T)(serializedObject)
            End If

            Return Nothing
        End Function

        Public Shared Sub SaveObject(Of T)(ByVal key As String, ByVal objectToSave As T)
            Dim serializedObject As String = Serialize(objectToSave)
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            localSettings.Values(key) = serializedObject
        End Sub

        Public Shared Sub DeleteObject(ByVal key As String)
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
            localSettings.Values.Remove(key)
        End Sub

        Public Shared Function Serialize(ByVal objectToSerialize As Object) As String
            Using ms As New MemoryStream()
                Dim serializer As New DataContractJsonSerializer(objectToSerialize.GetType())
                serializer.WriteObject(ms, objectToSerialize)
                ms.Position = 0

                Using reader As New StreamReader(ms)
                    Return reader.ReadToEnd()
                End Using
            End Using
        End Function

        Public Shared Function Deserialize(Of T)(ByVal jsonString As String) As T
            Using ms As New MemoryStream(Encoding.Unicode.GetBytes(jsonString))
                Dim serializer As New DataContractJsonSerializer(GetType(T))
                Return CType(serializer.ReadObject(ms), T)
            End Using
        End Function
    End Class

End Namespace