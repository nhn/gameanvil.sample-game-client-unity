// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Authentication.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Com.Nhn.Gameanvil.Sample.Protocol {

  /// <summary>Holder for reflection information generated from Authentication.proto</summary>
  public static partial class AuthenticationReflection {

    #region Descriptor
    /// <summary>File descriptor for Authentication.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static AuthenticationReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRBdXRoZW50aWNhdGlvbi5wcm90bxIhY29tLm5obi5nYW1lYW52aWwuc2Ft",
            "cGxlLnByb3RvY29sGgxSZXN1bHQucHJvdG8aClVzZXIucHJvdG8iKAoRQXV0",
            "aGVudGljYXRpb25SZXESEwoLYWNjZXNzVG9rZW4YASABKAkiVAoRQXV0aGVu",
            "dGljYXRpb25SZXMSPwoJZXJyb3JDb2RlGAEgASgOMiwuY29tLm5obi5nYW1l",
            "YW52aWwuc2FtcGxlLnByb3RvY29sLkVycm9yQ29kZSLDAQoITG9naW5SZXES",
            "DAoEdXVpZBgBIAEoCRI/Cglsb2dpblR5cGUYAiABKA4yLC5jb20ubmhuLmdh",
            "bWVhbnZpbC5zYW1wbGUucHJvdG9jb2wuTG9naW5UeXBlEhIKCmFwcFZlcnNp",
            "b24YAyABKAkSEAoIYXBwU3RvcmUYBCABKAkSEwoLZGV2aWNlTW9kZWwYBSAB",
            "KAkSFQoNZGV2aWNlQ291bnRyeRgGIAEoCRIWCg5kZXZpY2VMYW5ndWFnZRgH",
            "IAEoCSKKAQoITG9naW5SZXMSPwoJZXJyb3JDb2RlGAEgASgOMiwuY29tLm5o",
            "bi5nYW1lYW52aWwuc2FtcGxlLnByb3RvY29sLkVycm9yQ29kZRI9Cgh1c2Vy",
            "ZGF0YRgCIAEoCzIrLmNvbS5uaG4uZ2FtZWFudmlsLnNhbXBsZS5wcm90b2Nv",
            "bC5Vc2VyRGF0YSosCglMb2dpblR5cGUSDgoKTE9HSU5fTk9ORRAAEg8KC0xP",
            "R0lOX0dVRVNUEAFiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Com.Nhn.Gameanvil.Sample.Protocol.ResultReflection.Descriptor, global::Com.Nhn.Gameanvil.Sample.Protocol.UserReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Com.Nhn.Gameanvil.Sample.Protocol.LoginType), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReq), global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReq.Parser, new[]{ "AccessToken" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationRes), global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationRes.Parser, new[]{ "ErrorCode" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Nhn.Gameanvil.Sample.Protocol.LoginReq), global::Com.Nhn.Gameanvil.Sample.Protocol.LoginReq.Parser, new[]{ "Uuid", "LoginType", "AppVersion", "AppStore", "DeviceModel", "DeviceCountry", "DeviceLanguage" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Nhn.Gameanvil.Sample.Protocol.LoginRes), global::Com.Nhn.Gameanvil.Sample.Protocol.LoginRes.Parser, new[]{ "ErrorCode", "Userdata" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  /// <summary>
  /// 클라이언트 로그인 종류
  /// </summary>
  public enum LoginType {
    [pbr::OriginalName("LOGIN_NONE")] LoginNone = 0,
    [pbr::OriginalName("LOGIN_GUEST")] LoginGuest = 1,
  }

  #endregion

  #region Messages
  /// <summary>
  /// 인증 요청
  /// </summary>
  public sealed partial class AuthenticationReq : pb::IMessage<AuthenticationReq> {
    private static readonly pb::MessageParser<AuthenticationReq> _parser = new pb::MessageParser<AuthenticationReq>(() => new AuthenticationReq());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AuthenticationReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AuthenticationReq() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AuthenticationReq(AuthenticationReq other) : this() {
      accessToken_ = other.accessToken_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AuthenticationReq Clone() {
      return new AuthenticationReq(this);
    }

    /// <summary>Field number for the "accessToken" field.</summary>
    public const int AccessTokenFieldNumber = 1;
    private string accessToken_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string AccessToken {
      get { return accessToken_; }
      set {
        accessToken_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AuthenticationReq);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AuthenticationReq other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (AccessToken != other.AccessToken) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (AccessToken.Length != 0) hash ^= AccessToken.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (AccessToken.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(AccessToken);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (AccessToken.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AccessToken);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AuthenticationReq other) {
      if (other == null) {
        return;
      }
      if (other.AccessToken.Length != 0) {
        AccessToken = other.AccessToken;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            AccessToken = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class AuthenticationRes : pb::IMessage<AuthenticationRes> {
    private static readonly pb::MessageParser<AuthenticationRes> _parser = new pb::MessageParser<AuthenticationRes>(() => new AuthenticationRes());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AuthenticationRes> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AuthenticationRes() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AuthenticationRes(AuthenticationRes other) : this() {
      errorCode_ = other.errorCode_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AuthenticationRes Clone() {
      return new AuthenticationRes(this);
    }

    /// <summary>Field number for the "errorCode" field.</summary>
    public const int ErrorCodeFieldNumber = 1;
    private global::Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode errorCode_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode ErrorCode {
      get { return errorCode_; }
      set {
        errorCode_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AuthenticationRes);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AuthenticationRes other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ErrorCode != other.ErrorCode) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ErrorCode != 0) hash ^= ErrorCode.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ErrorCode != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) ErrorCode);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ErrorCode != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ErrorCode);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AuthenticationRes other) {
      if (other == null) {
        return;
      }
      if (other.ErrorCode != 0) {
        ErrorCode = other.ErrorCode;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            errorCode_ = (global::Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 로그인 요청
  /// </summary>
  public sealed partial class LoginReq : pb::IMessage<LoginReq> {
    private static readonly pb::MessageParser<LoginReq> _parser = new pb::MessageParser<LoginReq>(() => new LoginReq());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<LoginReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginReq() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginReq(LoginReq other) : this() {
      uuid_ = other.uuid_;
      loginType_ = other.loginType_;
      appVersion_ = other.appVersion_;
      appStore_ = other.appStore_;
      deviceModel_ = other.deviceModel_;
      deviceCountry_ = other.deviceCountry_;
      deviceLanguage_ = other.deviceLanguage_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginReq Clone() {
      return new LoginReq(this);
    }

    /// <summary>Field number for the "uuid" field.</summary>
    public const int UuidFieldNumber = 1;
    private string uuid_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Uuid {
      get { return uuid_; }
      set {
        uuid_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "loginType" field.</summary>
    public const int LoginTypeFieldNumber = 2;
    private global::Com.Nhn.Gameanvil.Sample.Protocol.LoginType loginType_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Com.Nhn.Gameanvil.Sample.Protocol.LoginType LoginType {
      get { return loginType_; }
      set {
        loginType_ = value;
      }
    }

    /// <summary>Field number for the "appVersion" field.</summary>
    public const int AppVersionFieldNumber = 3;
    private string appVersion_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string AppVersion {
      get { return appVersion_; }
      set {
        appVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "appStore" field.</summary>
    public const int AppStoreFieldNumber = 4;
    private string appStore_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string AppStore {
      get { return appStore_; }
      set {
        appStore_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "deviceModel" field.</summary>
    public const int DeviceModelFieldNumber = 5;
    private string deviceModel_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string DeviceModel {
      get { return deviceModel_; }
      set {
        deviceModel_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "deviceCountry" field.</summary>
    public const int DeviceCountryFieldNumber = 6;
    private string deviceCountry_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string DeviceCountry {
      get { return deviceCountry_; }
      set {
        deviceCountry_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "deviceLanguage" field.</summary>
    public const int DeviceLanguageFieldNumber = 7;
    private string deviceLanguage_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string DeviceLanguage {
      get { return deviceLanguage_; }
      set {
        deviceLanguage_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as LoginReq);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(LoginReq other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Uuid != other.Uuid) return false;
      if (LoginType != other.LoginType) return false;
      if (AppVersion != other.AppVersion) return false;
      if (AppStore != other.AppStore) return false;
      if (DeviceModel != other.DeviceModel) return false;
      if (DeviceCountry != other.DeviceCountry) return false;
      if (DeviceLanguage != other.DeviceLanguage) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Uuid.Length != 0) hash ^= Uuid.GetHashCode();
      if (LoginType != 0) hash ^= LoginType.GetHashCode();
      if (AppVersion.Length != 0) hash ^= AppVersion.GetHashCode();
      if (AppStore.Length != 0) hash ^= AppStore.GetHashCode();
      if (DeviceModel.Length != 0) hash ^= DeviceModel.GetHashCode();
      if (DeviceCountry.Length != 0) hash ^= DeviceCountry.GetHashCode();
      if (DeviceLanguage.Length != 0) hash ^= DeviceLanguage.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uuid.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Uuid);
      }
      if (LoginType != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) LoginType);
      }
      if (AppVersion.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(AppVersion);
      }
      if (AppStore.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(AppStore);
      }
      if (DeviceModel.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(DeviceModel);
      }
      if (DeviceCountry.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(DeviceCountry);
      }
      if (DeviceLanguage.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(DeviceLanguage);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uuid.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Uuid);
      }
      if (LoginType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) LoginType);
      }
      if (AppVersion.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AppVersion);
      }
      if (AppStore.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AppStore);
      }
      if (DeviceModel.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(DeviceModel);
      }
      if (DeviceCountry.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(DeviceCountry);
      }
      if (DeviceLanguage.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(DeviceLanguage);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(LoginReq other) {
      if (other == null) {
        return;
      }
      if (other.Uuid.Length != 0) {
        Uuid = other.Uuid;
      }
      if (other.LoginType != 0) {
        LoginType = other.LoginType;
      }
      if (other.AppVersion.Length != 0) {
        AppVersion = other.AppVersion;
      }
      if (other.AppStore.Length != 0) {
        AppStore = other.AppStore;
      }
      if (other.DeviceModel.Length != 0) {
        DeviceModel = other.DeviceModel;
      }
      if (other.DeviceCountry.Length != 0) {
        DeviceCountry = other.DeviceCountry;
      }
      if (other.DeviceLanguage.Length != 0) {
        DeviceLanguage = other.DeviceLanguage;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Uuid = input.ReadString();
            break;
          }
          case 16: {
            loginType_ = (global::Com.Nhn.Gameanvil.Sample.Protocol.LoginType) input.ReadEnum();
            break;
          }
          case 26: {
            AppVersion = input.ReadString();
            break;
          }
          case 34: {
            AppStore = input.ReadString();
            break;
          }
          case 42: {
            DeviceModel = input.ReadString();
            break;
          }
          case 50: {
            DeviceCountry = input.ReadString();
            break;
          }
          case 58: {
            DeviceLanguage = input.ReadString();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 로그인 응답
  /// </summary>
  public sealed partial class LoginRes : pb::IMessage<LoginRes> {
    private static readonly pb::MessageParser<LoginRes> _parser = new pb::MessageParser<LoginRes>(() => new LoginRes());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<LoginRes> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginRes() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginRes(LoginRes other) : this() {
      errorCode_ = other.errorCode_;
      userdata_ = other.userdata_ != null ? other.userdata_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginRes Clone() {
      return new LoginRes(this);
    }

    /// <summary>Field number for the "errorCode" field.</summary>
    public const int ErrorCodeFieldNumber = 1;
    private global::Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode errorCode_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode ErrorCode {
      get { return errorCode_; }
      set {
        errorCode_ = value;
      }
    }

    /// <summary>Field number for the "userdata" field.</summary>
    public const int UserdataFieldNumber = 2;
    private global::Com.Nhn.Gameanvil.Sample.Protocol.UserData userdata_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Com.Nhn.Gameanvil.Sample.Protocol.UserData Userdata {
      get { return userdata_; }
      set {
        userdata_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as LoginRes);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(LoginRes other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ErrorCode != other.ErrorCode) return false;
      if (!object.Equals(Userdata, other.Userdata)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ErrorCode != 0) hash ^= ErrorCode.GetHashCode();
      if (userdata_ != null) hash ^= Userdata.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ErrorCode != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) ErrorCode);
      }
      if (userdata_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Userdata);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ErrorCode != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ErrorCode);
      }
      if (userdata_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Userdata);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(LoginRes other) {
      if (other == null) {
        return;
      }
      if (other.ErrorCode != 0) {
        ErrorCode = other.ErrorCode;
      }
      if (other.userdata_ != null) {
        if (userdata_ == null) {
          userdata_ = new global::Com.Nhn.Gameanvil.Sample.Protocol.UserData();
        }
        Userdata.MergeFrom(other.Userdata);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            errorCode_ = (global::Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode) input.ReadEnum();
            break;
          }
          case 18: {
            if (userdata_ == null) {
              userdata_ = new global::Com.Nhn.Gameanvil.Sample.Protocol.UserData();
            }
            input.ReadMessage(userdata_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code