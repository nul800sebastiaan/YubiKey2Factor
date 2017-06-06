angular.module("umbraco.services").factory("twoFactorService", function ($http) {
    return {
        getEnabled: function (userId) {
            return $http.get("/umbraco/backoffice/api/TwoFactorAuth/TwoFactorEnabled/?userId=" + userId);
        },
        getGoogleAuthenticatorSetupCode: function () {
            return $http.get("/umbraco/backoffice/api/TwoFactorAuth/GoogleAuthenticatorSetupCode/");
        },
        validateAndSave: function (code) {
            return $http.post("/umbraco/backoffice/api/TwoFactorAuth/ValidateAndSave/?code=" + code);
        },
        validateAndSaveGoogleAuth: function (code) {
            return $http.post("/umbraco/backoffice/api/TwoFactorAuth/ValidateAndSaveGoogleAuth/?code=" + code);
        },
        disable: function () {
            return $http.post("/umbraco/backoffice/api/TwoFactorAuth/Disable/");
        }
    };
});