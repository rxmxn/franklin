"use strict";
angular.module("abcadmin")

.factory("sessionsFact", function () {
    return {
        //obtenemos una sesión //getter
        get: function (key) {
            return sessionStorage.getItem(key) === "true" ? true : false;
        },
        //creamos una sesión //setter
        set: function (key, val) {
            return sessionStorage.setItem(key, val);
        },
        //limpiamos una sesión
        unset: function (key) {
            return sessionStorage.removeItem(key);
        }
    }
})

.factory('SecurityFact', ['$http', '$q', '$location', 'sessionsFact', "Notification", "$rootScope", 
    function ($http, $q, $location, sessionsFact, notification, $rootScope) {
        var employee = null;
        var roleAccess;
	    var columnList;
		
        var cacheSession = function (username) {
            sessionsFact.set("userLogin", true);
            sessionsFact.set("username", username);
        }
        var unCacheSession = function () {
            sessionsFact.unset("userLogin");
            sessionsFact.unset("username");
        }

        function inArray(valor, array) {
            for (var key in array) {
                if (array.hasOwnProperty(key)) {
                    if (valor.indexOf(array[key]) > -1) {
                        return true;
                    }
                }
            }
            return false;
        }

        function urlContains(url, word) {
        	if (url.indexOf(word) > -1) {
        		return true;
        	}
        	return false;
        }

	    //function checkUser() {
	    //	$http.post('/security/CheckUserConnection')
        //    .success(function (response) {
        //    	if (response.success === true) {
        //    		employee = response.user;
        //    		roleAccess = response.accessList;
        //    		columnList = response.columnList;
        //    		$rootScope.$emit('page:refresh');
        //    		return true;
        //    	} else {
        //    		return false;
        //    	}
        //    })
        //    .error(function () {
        //    	return false;
        //    });
	    //}

        return {
            subscribe: function(scope, callback) {
                var handler = $rootScope.$on('page:refresh', callback);
                scope.$on('$destroy', handler);
            },
            
            isLogged: function () {
                return sessionsFact.get("userLogin");
            },

            login: function (user, pass) {
                var deferredObject = $q.defer();

                $http.post('/security/userlogin', { uname: user, upass: pass })
                .success(function (response) {
                    if (response.success === true) {
                        employee = response.user;
                        roleAccess = response.accessList;
                        columnList = response.columnList;
                        cacheSession(response.user.UserName);
                        $rootScope.$emit('page:refresh');
                        deferredObject.resolve({ success: true, employee: response.user });
                    } else {
                        deferredObject.resolve({ success: false });
                    }
                })
                .error(function () {
                    deferredObject.resolve({ success: false });
                });
                return deferredObject.promise;
            },

            checkAuth: function () {
                // If this routes are put in the URL without authentication or the proper access level 
                // then the user will be redirected to his home page.
            	var privateRoute = "/system";
            	var menuRoutes = ["/charts", "/access", "/audit", "/massivetransfer"];
                var adminRoutes = ["/user", "/role"];   //al crear los roles hay que ponerlos mas generales. Ejemplo, el de admin que abarque user y role.
                var methodRoutes = ["/method", "/container", "/preserver", "/residue", "/analyticsmethod", "/material"];
                var paramRoutes = ["/parameter", "/groups", "packages", "/norms", "/currency"];
                var annalistRoutes = ["/annalist", "/analyticsarea", "/centrocosto", "/analyticsunit"];
                var ackRoutes = ["/ack", "/action", "/enterprises"];
                var baseparamRoutes = ["/physicalparam", "/measureunits"];
                var matrixRoute = ["/matrixes", "/basematrixes"];
                var regionRoute = ["/region", "/sucursal"];
                var cqRoute = ["/chemicalclasification1", "/chemicalclasification2", "/chemicalclasification3"];	// clasificacion quimica
                var otherRoutes = ["/servicetypes", "/branch"];
                
                // SECURITY
                if (this.isLogged()) {
                    // si el usuario esta logueado se verifican sus permisos
                	$http.post('/security/CheckUserConnection')
					.success(function (response) {
            			if (response.success === true) {
            				employee = response.user;
            				roleAccess = response.accessList;
            				columnList = response.columnList;
            				$rootScope.$emit('page:refresh');
            				
            				if (($location.path() === "/login/") ||
								(urlContains($location.path(), menuRoutes[0]) && (roleAccess["charts"] === -1)) ||
								(urlContains($location.path(), menuRoutes[1]) && (roleAccess["useraccess"] === -1)) ||
								(urlContains($location.path(), menuRoutes[2]) && (roleAccess["audit"] === -1)) ||
								(urlContains($location.path(), menuRoutes[3]) && (roleAccess["massivetransfer"] === -1)) ||
								(urlContains($location.path(), adminRoutes[0]) && (roleAccess["user"] === -1)) ||
								(urlContains($location.path(), adminRoutes[1]) && (roleAccess["role"] === -1)) ||
								(urlContains($location.path(), methodRoutes[0]) && (roleAccess["method"] === -1)) ||
								(urlContains($location.path(), methodRoutes[1]) && (roleAccess["container"] === -1)) ||
								(urlContains($location.path(), methodRoutes[2]) && (roleAccess["preserver"] === -1)) ||
								(urlContains($location.path(), methodRoutes[3]) && (roleAccess["residue"] === -1)) ||
								(urlContains($location.path(), methodRoutes[4]) && (roleAccess["analyticsmethod"] === -1)) ||
								(urlContains($location.path(), methodRoutes[5]) && (roleAccess["material"] === -1)) ||
								(urlContains($location.path(), paramRoutes[0]) && (roleAccess["param"] === -1)) ||
								(urlContains($location.path(), paramRoutes[1]) && (roleAccess["group"] === -1)) ||
								(urlContains($location.path(), paramRoutes[2]) && (roleAccess["package"] === -1)) ||
								(urlContains($location.path(), paramRoutes[3]) && (roleAccess["norm"] === -1)) ||
								(urlContains($location.path(), paramRoutes[4]) && (roleAccess["currency"] === -1)) ||
								(urlContains($location.path(), annalistRoutes[0]) && (roleAccess["annalist"] === -1)) ||
								(urlContains($location.path(), annalistRoutes[1]) && (roleAccess["analyticsarea"] === -1)) ||
								(urlContains($location.path(), annalistRoutes[2]) && (roleAccess["analyticsarea"] === -1)) ||
								(urlContains($location.path(), annalistRoutes[3]) && (roleAccess["analyticsarea"] === -1)) ||
								(inArray($location.path(), ackRoutes) && (roleAccess["ack"] === -1)) ||
								(urlContains($location.path(), baseparamRoutes[0]) && (roleAccess["baseparam"] === -1) && !urlContains($location.path(), baseparamRoutes[1])) ||
								(urlContains($location.path(), baseparamRoutes[1]) && (roleAccess["measureunits"] === -1)) ||
								(urlContains($location.path(), matrixRoute[0]) && (roleAccess["matrix"] === -1)) ||
								(urlContains($location.path(), matrixRoute[1]) && (roleAccess["basematrix"] === -1)) ||
								(urlContains($location.path(), regionRoute[0]) && (roleAccess["region"] === -1)) || 
								(urlContains($location.path(), regionRoute[1]) && (roleAccess["region"] === -1)) ||
								(urlContains($location.path(), cqRoute[0]) && (roleAccess["clasificacionquimica"] === -1)) ||
								(urlContains($location.path(), cqRoute[1]) && (roleAccess["clasificacionquimica"] === -1)) ||
								(urlContains($location.path(), cqRoute[2]) && (roleAccess["clasificacionquimica"] === -1)) ||
								(urlContains($location.path(), otherRoutes[0]) && (roleAccess["tiposervicio"] === -1)) ||
								(urlContains($location.path(), otherRoutes[1]) && (roleAccess["rama"] === -1)))
            				{
            					$location.path("/system/home/");
            				}

            			} else {
            				notification.error({
            					message: "Ha transcurrido bastante tiempo desde que dejó de usar su sesión. Por seguridad será enviado a la página LOGIN.",
            					delay: 5000
            				});
                            employee = null;
                            unCacheSession();
                            $location.path("/login/");
            			}
					})
					.error(function () {
						notification.error({
							message: "Error de comunicación con el servidor.",
							delay: 5000
						});
					});
                } else {
                    // si el usuario trata de acceder a una ruta privada y no esta logueado se redirecciona a login
                	if (urlContains($location.path(), privateRoute))
                        $location.path("/login/");
                }
            },

            logout: function () {
                $http.post('/security/userlogout')
                .success(function () {
                    employee = null;
                    unCacheSession();
                    $location.path('/login/');
                })
                .error(function() {
                    notification.error({
                        message: "Error al desloguear usuario.",
                        delay: 5000
                    });
                });
            },

            employeeDataExpress: function () {
            	return employee;
            },

            checkAccessLevel: function (value) {
            	if (roleAccess !== undefined) {
            		return roleAccess[value];
	            }
	            return null;
            },

			checkAllAccessLevel: function() {
				return roleAccess;
			},

			getColumnList: function () {
				return columnList;
			}
        }
    }])

.factory("authResponseInterceptor", ["$q", "$window", "$location", function ($q, $window, $location) {
    var isloading;
    return {
        request: function (config) {
            if (config.method === "GET") {
                isloading = true;
                //$("#workregion").hide();
                $("#loadingSpinners").show(); // usarlo manualmente cuando se necesite.               
            }
            if (!isloading && config.method === "POST") {
                $("#loadingSpinners").hide();
                $("#workregion").show();
            }
            // if (config.method === "GET" || config.method === "POST")
            //     $("#loadingSpinners").show(); 

            return config || $q.when(config);
        },
        requestError: function (rejection) {
            $("#loadingSpinners").hide();
            $("#workregion").show();
            return $q.reject(rejection);
        },
        response: function (response) {
            //console.log(response);
            $("#loadingSpinners").hide();
            $("#workregion").show();
            return response || $q.when(response);
        },
        responseError: function (rejection) {
            if (rejection.status === 401) {
                $location.path("/login/");
            }
            return $q.reject(rejection);
        }
    }
}])

;