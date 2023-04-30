angular.module("abcadmin")
    .controller("LoginController", ["$scope", "$location", "SecurityFact", "Notification", 'counterFactory',
        function ($scope, $location, securityFact, notification, counterFactory) {
            
            $scope.loginBtnText = "Entrar";
            $scope.disabledBtn = ""; //change for "disabled"
            $scope.loginState = false;

            $scope.login = function () {
                $scope.loginState = true;
                $scope.loginBtnText = "Verificando...";
                $scope.disabledBtn = "disabled";
                
                var response = securityFact.login($scope.credentials.username, $scope.credentials.password);
                response.then(function (response) {
                    if (response.success) {
                        $scope.disabledBtn = "";
                        $scope.loginState = false;
                        $scope.loginBtnText = "Listo!";

                        $location.path("/system/home/");

                        notification.success({ message: "Sesión iniciada", delay: 3000 });
                    } else {
                        notification.error({
                            message: "Compruebe que los datos sean correctos.", delay: 3000
                        });
                        $scope.disabledBtn = "";
                        $scope.loginState = false;
                        $scope.loginBtnText = "Entrar";
                    }
                });
            };

            $scope.cancelLog = function () {
                $scope.credentials.username = "";
                $scope.credentials.password = "";
            };

        }]);