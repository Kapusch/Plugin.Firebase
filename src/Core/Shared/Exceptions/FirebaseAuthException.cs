namespace Plugin.Firebase.Core.Exceptions;

/// <summary>
/// Cross-platform Firebase Auth error reasons.
/// </summary>
public enum FIRAuthError
{
    /// <summary>
    /// Unknown error reason.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Indicates the email address is malformed.
    /// </summary>
    InvalidEmail = 1,

    /// <summary>
    /// Indicates the user attempted sign in with a wrong password.
    /// </summary>
    WrongPassword = 2,

    /// <summary>
    /// Indicates an attempt to set a password that is considered too weak.
    /// </summary>
    WeakPassword = 3,

    /// <summary>
    /// Indicates the email used to attempt sign up already exists.
    /// </summary>
    EmailAlreadyInUse = 4,

    /// <summary>
    /// Indicates the user account was not found.
    /// </summary>
    UserNotFound = 5,

    /// <summary>
    /// Indicates the current user’s token has expired, for example, the user may have changed account password on another device.
    /// You must prompt the user to sign in again on this device.
    /// </summary>
    UserTokenExpired = 6,

    /// <summary>
    /// Indicates the supplied credential is invalid. This could happen if it has expired or it is malformed.
    /// </summary>
    InvalidCredential = 7,

    /// <summary>
    /// Indicates the user's account is disabled.
    /// </summary>
    UserDisabled = 8,

    /// <summary>
    /// Indicates the user already signed in once with a trusted provider and hence cannot sign in with another provider.
    /// List of trusted and untrusted providers: https://firebase.google.com/docs/auth/users#verified_email_addresses
    /// </summary>
    AccountExistsWithDifferentCredential = 9,

    /// <summary>
    /// Indicates a validation error with the custom token.
    /// </summary>
    InvalidCustomToken = 10,

    /// <summary>
    /// Indicates the service account and the API key belong to different projects.
    /// </summary>
    CustomTokenMismatch = 11,

    /// <summary>
    /// Indicates the saved auth credential is invalid. The user needs to sign in again.
    /// </summary>
    InvalidUserToken = 12,

    /// <summary>
    /// Indicates that the operation is not allowed (provider disabled or email/password not enabled).
    /// </summary>
    OperationNotAllowed = 13,

    /// <summary>
    /// Indicates that the user must reauthenticate due to a recent-login requirement.
    /// </summary>
    RequiresRecentLogin = 14,

    /// <summary>
    /// Indicates that a different user than the current user was used for reauthentication.
    /// </summary>
    UserMismatch = 15,

    /// <summary>
    /// Indicates that the provider has already been linked to the user.
    /// </summary>
    ProviderAlreadyLinked = 16,

    /// <summary>
    /// Indicates the provider is not linked to the user.
    /// </summary>
    NoSuchProvider = 17,

    /// <summary>
    /// Indicates the email asserted by a credential is already in use by another account.
    /// </summary>
    CredentialAlreadyInUse = 18,

    /// <summary>
    /// Indicates that the phone auth credential was created with an empty verification ID.
    /// </summary>
    MissingVerificationId = 19,

    /// <summary>
    /// Indicates that the phone auth credential was created with an empty verification code.
    /// </summary>
    MissingVerificationCode = 20,

    /// <summary>
    /// Indicates that the phone auth credential was created with an invalid verification code.
    /// </summary>
    InvalidVerificationCode = 21,

    /// <summary>
    /// Indicates that the phone auth credential was created with an invalid verification ID.
    /// </summary>
    InvalidVerificationId = 22,

    /// <summary>
    /// Indicates that the SMS code has expired.
    /// </summary>
    SessionExpired = 23,

    /// <summary>
    /// Indicates that the quota of SMS messages for a given project has been exceeded.
    /// </summary>
    QuotaExceeded = 24,

    /// <summary>
    /// Indicates that the APNs device token was not obtained or forwarded.
    /// </summary>
    MissingAppToken = 25,

    /// <summary>
    /// Indicates that the APNs device token was missing when required for phone auth.
    /// </summary>
    MissingAppCredential = 26,

    /// <summary>
    /// Indicates that an invalid APNs device token was used.
    /// </summary>
    InvalidAppCredential = 27,

    /// <summary>
    /// Indicates that a notification was not forwarded to Firebase Auth when required.
    /// </summary>
    NotificationNotForwarded = 28,

    /// <summary>
    /// Indicates an invalid recipient email was sent in the request.
    /// </summary>
    InvalidRecipientEmail = 29,

    /// <summary>
    /// Indicates an invalid sender email is set in the console for this action.
    /// </summary>
    InvalidSender = 30,

    /// <summary>
    /// Indicates an invalid email template for sending update email.
    /// </summary>
    InvalidMessagePayload = 31,

    /// <summary>
    /// Indicates that the iOS bundle ID is missing when required.
    /// </summary>
    MissingIosBundleId = 32,

    /// <summary>
    /// Indicates that the Android package name is missing when required.
    /// </summary>
    MissingAndroidPackageName = 33,

    /// <summary>
    /// Indicates that the domain specified in the continue URL is not allowlisted.
    /// </summary>
    UnauthorizedDomain = 34,

    /// <summary>
    /// Indicates that the domain specified in the continue URL is not valid.
    /// </summary>
    InvalidContinueUri = 35,

    /// <summary>
    /// Indicates an invalid API key was supplied in the request.
    /// </summary>
    InvalidApiKey = 36,

    /// <summary>
    /// Indicates the app is not authorized to use Firebase Authentication with the provided API key.
    /// </summary>
    AppNotAuthorized = 37,

    /// <summary>
    /// Indicates an error occurred when accessing the keychain.
    /// </summary>
    KeychainError = 38,

    /// <summary>
    /// Indicates an internal error occurred.
    /// </summary>
    InternalError = 39,

    /// <summary>
    /// Indicates a network error occurred during the operation.
    /// </summary>
    NetworkError = 40,

    /// <summary>
    /// Indicates the request has been blocked after an abnormal number of requests.
    /// </summary>
    TooManyRequests = 41,

    /// <summary>
    /// Indicates a web-based sign-in flow failed due to a web context network error.
    /// </summary>
    WebNetworkRequestFailed = 42,
}

/// <summary>
/// Exception representing an authentication failure with a normalized <see cref="FIRAuthError"/> reason.
/// </summary>
public class FirebaseAuthException : FirebaseException
{
    /// <summary>
    /// Normalized authentication error reason.
    /// </summary>
    public FIRAuthError Reason { get; }

    /// <summary>
    /// Gets the error code string that provides additional information about the error.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Native error domain, when available.
    /// </summary>
    public string? NativeErrorDomain { get; }

    /// <summary>
    /// Native error code, when available.
    /// </summary>
    public long? NativeErrorCode { get; }

    /// <summary>
    /// Email address involved in the failure, when available.
    /// </summary>
    public string? Email { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="reason">Normalized authentication error reason.</param>
    public FirebaseAuthException(FIRAuthError reason)
        : this(reason, string.Empty) { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="reason">Normalized authentication error reason.</param>
    /// <param name="message">Error message.</param>
    public FirebaseAuthException(FIRAuthError reason, string message)
        : this(reason, message, null, null, null, null, null) { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="reason">Normalized authentication error reason.</param>
    /// <param name="message">Error message.</param>
    /// <param name="inner">Inner exception.</param>
    public FirebaseAuthException(FIRAuthError reason, string message, Exception inner)
        : this(reason, message, inner, null, null, null, null) { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="reason">Normalized authentication error reason.</param>
    /// <param name="message">Error message.</param>
    /// <param name="inner">Inner exception.</param>
    /// <param name="errorCode">Native/provider error code string, when available.</param>
    /// <param name="nativeErrorDomain">Native error domain, when available.</param>
    /// <param name="nativeErrorCode">Native error code, when available.</param>
    /// <param name="email">Email involved in the failure, when available.</param>
    public FirebaseAuthException(
        FIRAuthError reason,
        string message,
        Exception? inner,
        string? errorCode,
        string? nativeErrorDomain,
        long? nativeErrorCode,
        string? email
    )
        : base(message, inner)
    {
        Reason = reason;
        ErrorCode = errorCode;
        NativeErrorDomain = nativeErrorDomain;
        NativeErrorCode = nativeErrorCode;
        Email = email;
    }

    /// <summary>
    /// Creates a <see cref="FirebaseAuthException"/> from a provider/native error code and related metadata.
    /// </summary>
    /// <param name="errorCode">Native/provider error code string, when available.</param>
    /// <param name="message">Error message.</param>
    /// <param name="inner">Inner exception.</param>
    /// <param name="nativeErrorDomain">Native error domain, when available.</param>
    /// <param name="nativeErrorCode">Native error code, when available.</param>
    /// <param name="email">Email involved in the failure, when available.</param>
    /// <returns>A new <see cref="FirebaseAuthException"/>.</returns>
    public static FirebaseAuthException FromErrorCode(
        string? errorCode,
        string? message,
        Exception? inner = null,
        string? nativeErrorDomain = null,
        long? nativeErrorCode = null,
        string? email = null
    )
    {
        var reason = MapReason(errorCode);
        return new FirebaseAuthException(
            reason,
            message ?? string.Empty,
            inner,
            errorCode,
            nativeErrorDomain,
            nativeErrorCode,
            email
        );
    }

    private static FIRAuthError MapReason(string? errorCode)
    {
        if(string.IsNullOrWhiteSpace(errorCode)) {
            return FIRAuthError.Undefined;
        }

        var normalized = NormalizeErrorCode(errorCode);
        if(IsNumericCode(normalized)) {
            return FIRAuthError.Undefined;
        }

        if(string.Equals(normalized, "NetworkRequestFailed", StringComparison.OrdinalIgnoreCase)) {
            return FIRAuthError.NetworkError;
        }

        if(
            Enum.TryParse(normalized, ignoreCase: true, out FIRAuthError reason)
            && Enum.IsDefined(typeof(FIRAuthError), reason)
        ) {
            return reason;
        }

        return FIRAuthError.Undefined;
    }

    private static string NormalizeErrorCode(string errorCode)
    {
        var code = errorCode.Trim();

        if(code.StartsWith("ERROR_", StringComparison.OrdinalIgnoreCase)) {
            code = code.Substring("ERROR_".Length);
        }

        if(code.StartsWith("FIRAuthErrorCode", StringComparison.OrdinalIgnoreCase)) {
            code = code.Substring("FIRAuthErrorCode".Length);
        }

        if(code.StartsWith("AuthErrorCode", StringComparison.OrdinalIgnoreCase)) {
            code = code.Substring("AuthErrorCode".Length);
        }

        if(code.Contains('_')) {
            return ToPascalCase(code);
        }

        return code;
    }

    private static bool IsNumericCode(string code)
    {
        return long.TryParse(
            code,
            System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture,
            out _
        );
    }

    private static string ToPascalCase(string code)
    {
        var parts = code.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if(parts.Length == 0) {
            return code;
        }

        var buffer = new System.Text.StringBuilder(code.Length);
        foreach(var part in parts) {
            if(part.Length == 0) {
                continue;
            }

            var lower = part.ToLowerInvariant();
            buffer.Append(char.ToUpperInvariant(lower[0]));
            if(lower.Length > 1) {
                buffer.Append(lower.Substring(1));
            }
        }

        return buffer.ToString();
    }
}