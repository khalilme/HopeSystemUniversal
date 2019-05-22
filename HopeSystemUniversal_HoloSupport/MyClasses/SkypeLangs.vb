Public Class SkypeLangs
    Inherits List(Of Three)

    Sub New()
        Add(New Three("ar", "ar-EG-Hoda", "Arabic (ar-EG) (Egypt)"))
        Add(New Three("ca", "ca-ES-Herena", "Catalan (ca-ES) (Spain)"))
        Add(New Three("da", "da-DK-Helle", "Danish (da-DK) (Denmark)"))
        Add(New Three("de", "de-DE-Hedda", "German (de-DE) (Germany)"))
        Add(New Three("en", "en-AU-Catherine", "English (en-AU) (Australia)"))
        Add(New Three("es", "es-ES-HelenaRUS", "Spanish (es-ES) (Spain)"))
        Add(New Three("fi", "fi-FI-Heidi", "Finnish (fi-FI) (Finland)"))
        Add(New Three("fr", "fr-CA-Caroline", "French (fr-CA) (Canada)"))
        Add(New Three("hi", "hi-IN-Kalpana", "Hindi (hi-IN) (India)"))
        Add(New Three("it", "it-IT-Cosimo", "Italian (it-IT) (Italy)"))
        Add(New Three("ja", "ja-JP-Ayumi", "Japanese (ja-JP) (Japan)"))
        Add(New Three("ko", "ko-KR-HeamiRUS", "Korean (ko-KR) (Korea)"))
        Add(New Three("nb", "nb-NO-Jon", "Norwegian (nb-NO) (Norway)"))
        Add(New Three("nl", "nl-NL-Frank", "Dutch (nl-NL) (Netherlands)"))
        Add(New Three("pl", "pl-PL-Adam", "Polish (pl-PL) (Poland)"))
        Add(New Three("pt", "pt-BR-Daniel", "Portuguese (pt-BR) (Brazil)"))
        Add(New Three("ru", "ru-RU-Irina", "Russian (ru-RU) (Russia)"))
        Add(New Three("sv", "sv-SE-Bengt", "Swedish (sv-SE) (Sweden)"))
        Add(New Three("tr", "tr-TR-Seda", "Turkish (tr-TR) (Turkey)"))
        Add(New Three("zh-Hans", "zh-CN-HuihuiRUS", "Chinese Simplified (zh-CN) (People's Republic of China)"))
        Add(New Three("yue", "zh-HK-Danny", "Cantonese (Traditional) (zh-HK) (Hong Kong S.A.R.)"))
        Add(New Three("zh-Hant", "zh-TW-Yating", "Chinese Traditional (zh-TW) (Taiwan)"))
    End Sub

End Class
