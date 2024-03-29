﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFServiceWebRole.ATK.POS {
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ParserErrorCode", Namespace="http://nlptoolkit.cloudapp.net")]
    public enum ParserErrorCode : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorNonInitialized = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorAccessDenied = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorQuotaExceeded = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorAuthentication = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorMaxLengthExceeded = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorUnknown = 6,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ErrorInvalidParse = 7,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://nlptoolkit.cloudapp.net", ConfigurationName="ATK.POS.IParserService")]
    public interface IParserService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://nlptoolkit.cloudapp.net/IParserService/Parse", ReplyAction="http://nlptoolkit.cloudapp.net/IParserService/ParseResponse")]
        WCFServiceWebRole.ATK.POS.ParseResponse Parse(WCFServiceWebRole.ATK.POS.ParseRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://nlptoolkit.cloudapp.net/IParserService/Parse", ReplyAction="http://nlptoolkit.cloudapp.net/IParserService/ParseResponse")]
        System.Threading.Tasks.Task<WCFServiceWebRole.ATK.POS.ParseResponse> ParseAsync(WCFServiceWebRole.ATK.POS.ParseRequest request);
        
        // CODEGEN: Generating message contract since element name appId from namespace http://nlptoolkit.cloudapp.net is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://nlptoolkit.cloudapp.net/IParserService/SuggestParseTree", ReplyAction="http://nlptoolkit.cloudapp.net/IParserService/SuggestParseTreeResponse")]
        WCFServiceWebRole.ATK.POS.SuggestParseTreeResponse SuggestParseTree(WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://nlptoolkit.cloudapp.net/IParserService/SuggestParseTree", ReplyAction="http://nlptoolkit.cloudapp.net/IParserService/SuggestParseTreeResponse")]
        System.Threading.Tasks.Task<WCFServiceWebRole.ATK.POS.SuggestParseTreeResponse> SuggestParseTreeAsync(WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ParseRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Parse", Namespace="http://nlptoolkit.cloudapp.net", Order=0)]
        public WCFServiceWebRole.ATK.POS.ParseRequestBody Body;
        
        public ParseRequest() {
        }
        
        public ParseRequest(WCFServiceWebRole.ATK.POS.ParseRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://nlptoolkit.cloudapp.net")]
    public partial class ParseRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string appId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string inText;
        
        public ParseRequestBody() {
        }
        
        public ParseRequestBody(string appId, string inText) {
            this.appId = appId;
            this.inText = inText;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ParseResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ParseResponse", Namespace="http://nlptoolkit.cloudapp.net", Order=0)]
        public WCFServiceWebRole.ATK.POS.ParseResponseBody Body;
        
        public ParseResponse() {
        }
        
        public ParseResponse(WCFServiceWebRole.ATK.POS.ParseResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://nlptoolkit.cloudapp.net")]
    public partial class ParseResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public WCFServiceWebRole.ATK.POS.ParserErrorCode ParseResult;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string parseTree;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public double score;
        
        public ParseResponseBody() {
        }
        
        public ParseResponseBody(WCFServiceWebRole.ATK.POS.ParserErrorCode ParseResult, string parseTree, double score) {
            this.ParseResult = ParseResult;
            this.parseTree = parseTree;
            this.score = score;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SuggestParseTreeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SuggestParseTree", Namespace="http://nlptoolkit.cloudapp.net", Order=0)]
        public WCFServiceWebRole.ATK.POS.SuggestParseTreeRequestBody Body;
        
        public SuggestParseTreeRequest() {
        }
        
        public SuggestParseTreeRequest(WCFServiceWebRole.ATK.POS.SuggestParseTreeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://nlptoolkit.cloudapp.net")]
    public partial class SuggestParseTreeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string appId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string inText;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string alternateParseTree;
        
        public SuggestParseTreeRequestBody() {
        }
        
        public SuggestParseTreeRequestBody(string appId, string inText, string alternateParseTree) {
            this.appId = appId;
            this.inText = inText;
            this.alternateParseTree = alternateParseTree;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SuggestParseTreeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SuggestParseTreeResponse", Namespace="http://nlptoolkit.cloudapp.net", Order=0)]
        public WCFServiceWebRole.ATK.POS.SuggestParseTreeResponseBody Body;
        
        public SuggestParseTreeResponse() {
        }
        
        public SuggestParseTreeResponse(WCFServiceWebRole.ATK.POS.SuggestParseTreeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://nlptoolkit.cloudapp.net")]
    public partial class SuggestParseTreeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public WCFServiceWebRole.ATK.POS.ParserErrorCode SuggestParseTreeResult;
        
        public SuggestParseTreeResponseBody() {
        }
        
        public SuggestParseTreeResponseBody(WCFServiceWebRole.ATK.POS.ParserErrorCode SuggestParseTreeResult) {
            this.SuggestParseTreeResult = SuggestParseTreeResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IParserServiceChannel : WCFServiceWebRole.ATK.POS.IParserService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ParserServiceClient : System.ServiceModel.ClientBase<WCFServiceWebRole.ATK.POS.IParserService>, WCFServiceWebRole.ATK.POS.IParserService {
        
        public ParserServiceClient() {
        }
        
        public ParserServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ParserServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ParserServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ParserServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WCFServiceWebRole.ATK.POS.ParseResponse WCFServiceWebRole.ATK.POS.IParserService.Parse(WCFServiceWebRole.ATK.POS.ParseRequest request) {
            return base.Channel.Parse(request);
        }
        
        public WCFServiceWebRole.ATK.POS.ParserErrorCode Parse(string appId, string inText, out string parseTree, out double score) {
            WCFServiceWebRole.ATK.POS.ParseRequest inValue = new WCFServiceWebRole.ATK.POS.ParseRequest();
            inValue.Body = new WCFServiceWebRole.ATK.POS.ParseRequestBody();
            inValue.Body.appId = appId;
            inValue.Body.inText = inText;
            WCFServiceWebRole.ATK.POS.ParseResponse retVal = ((WCFServiceWebRole.ATK.POS.IParserService)(this)).Parse(inValue);
            parseTree = retVal.Body.parseTree;
            score = retVal.Body.score;
            return retVal.Body.ParseResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WCFServiceWebRole.ATK.POS.ParseResponse> WCFServiceWebRole.ATK.POS.IParserService.ParseAsync(WCFServiceWebRole.ATK.POS.ParseRequest request) {
            return base.Channel.ParseAsync(request);
        }
        
        public System.Threading.Tasks.Task<WCFServiceWebRole.ATK.POS.ParseResponse> ParseAsync(string appId, string inText) {
            WCFServiceWebRole.ATK.POS.ParseRequest inValue = new WCFServiceWebRole.ATK.POS.ParseRequest();
            inValue.Body = new WCFServiceWebRole.ATK.POS.ParseRequestBody();
            inValue.Body.appId = appId;
            inValue.Body.inText = inText;
            return ((WCFServiceWebRole.ATK.POS.IParserService)(this)).ParseAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WCFServiceWebRole.ATK.POS.SuggestParseTreeResponse WCFServiceWebRole.ATK.POS.IParserService.SuggestParseTree(WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest request) {
            return base.Channel.SuggestParseTree(request);
        }
        
        public WCFServiceWebRole.ATK.POS.ParserErrorCode SuggestParseTree(string appId, string inText, string alternateParseTree) {
            WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest inValue = new WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest();
            inValue.Body = new WCFServiceWebRole.ATK.POS.SuggestParseTreeRequestBody();
            inValue.Body.appId = appId;
            inValue.Body.inText = inText;
            inValue.Body.alternateParseTree = alternateParseTree;
            WCFServiceWebRole.ATK.POS.SuggestParseTreeResponse retVal = ((WCFServiceWebRole.ATK.POS.IParserService)(this)).SuggestParseTree(inValue);
            return retVal.Body.SuggestParseTreeResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WCFServiceWebRole.ATK.POS.SuggestParseTreeResponse> WCFServiceWebRole.ATK.POS.IParserService.SuggestParseTreeAsync(WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest request) {
            return base.Channel.SuggestParseTreeAsync(request);
        }
        
        public System.Threading.Tasks.Task<WCFServiceWebRole.ATK.POS.SuggestParseTreeResponse> SuggestParseTreeAsync(string appId, string inText, string alternateParseTree) {
            WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest inValue = new WCFServiceWebRole.ATK.POS.SuggestParseTreeRequest();
            inValue.Body = new WCFServiceWebRole.ATK.POS.SuggestParseTreeRequestBody();
            inValue.Body.appId = appId;
            inValue.Body.inText = inText;
            inValue.Body.alternateParseTree = alternateParseTree;
            return ((WCFServiceWebRole.ATK.POS.IParserService)(this)).SuggestParseTreeAsync(inValue);
        }
    }
}
