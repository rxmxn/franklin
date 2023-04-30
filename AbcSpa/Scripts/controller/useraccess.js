"use strict";

angular.module("abcadmin")
	.controller("UserAccessController",
	[
		"$scope", "$route", "$location", "$http", "$cookies", '$controller', "generalFactory",
		"SecurityFact", "Notification", "Fullscreen",
		function($scope, $route, $location, $http, $cookies, $controller, generalFactory,
			securityFact, notification, fullscreen) {

			$scope.isFullscreen0 = false;
			$scope.isFullscreen1 = false;
			//-----------función q se llama para mostrar un div en fullscreen----------------
			$scope.toggleFullScreen = function(index) {
				(index === 0) ? ($scope.isFullscreen0 = !$scope.isFullscreen0)
					: ($scope.isFullscreen1 = !$scope.isFullscreen1);
			}
			//---------------------------------------------------------------------------------
			$scope.currentPage = 1;
			$scope.pageSize = 10;
			$scope.searchGeneral = "";
            $scope.searchIp = "";

			var filters = function(page) {
				return {
					page: (page === null || page === undefined) ? $scope.currentPage : page,
					pageSize: $scope.pageSize,
					searchGeneral: $scope.searchGeneral,
					fromDate: $scope.fromDate === undefined ? null : $scope.fromDate,
					untilDate: $scope.untilDate === undefined ? null : $scope.untilDate,
					alta: $scope.alta,
					baja: $scope.baja,
                    searchIp: $scope.searchIp,
                    conected: $scope.viewConnected
				}
			};
			
			$scope.baja = false;
			$scope.alta = true;
            $scope.viewConnected = false;
			$scope.elementsList = null;
			$scope.isLoading = false;

			$scope.RefreshList = function(page) {
				$scope.isLoading = true;
				generalFactory.RefreshList("/useraccess/RefreshUserAccess", filters, page)
					.then(function(response) {
						if (response.success) {
							$scope.elementsList = response.elements;
							$scope.pageTotal = response.total;
							$scope.isLoading = false;
						} else {
							notification.error({
								message: "Error al recuperar los Elementos",
								delay: 5000
							});
							$scope.isLoading = false;
						}
					});
			}

			$scope.RefreshList(1);

			$scope.selectedUser = null;
			
			$scope.more = function (user) {
				$scope.selectedUser = user;
				$scope.SexoChange();
				$scope.panels[1].name = $scope.selectedUser.User.FullName;
				$scope.panels[1].state = false;
				$scope.minimize($scope.panels[1]);
				$scope.RefreshListMore(1);
			}
			
			$scope.currentPageMore = 1;
			$scope.pageSizeMore = 10;
            
            $scope.searchIpMore = "";

			var filtersMore = function (page) {
				return {
					page: (page === null || page === undefined) ? $scope.currentPageMore : page,
					pageSize: $scope.pageSizeMore,
					userId: $scope.selectedUser === undefined ? 0 : $scope.selectedUser.User.Id,
					fromDate: $scope.fromDateMore === undefined ? null : $scope.fromDateMore,
					untilDate: $scope.untilDateMore === undefined ? null : $scope.untilDateMore,
                    searchIp: $scope.searchIpMore
				}
			};

			$scope.elementsListMore = null;
			$scope.isLoadingMore = false;

			$scope.RefreshListMore = function (page) {
                if($scope.selectedUser !== null)
                {
                    $scope.isLoadingMore = true;
                    generalFactory.RefreshList("/useraccess/RefreshUserInfo", filtersMore, page)
                        .then(function (response) {
                            if (response.success) {
                                $scope.elementsListMore = response.elements;
                                $scope.pageTotalMore = response.total;
                                $scope.isLoadingMore = false;
                            } else {
                                notification.error({
                                    message: "Error al recuperar los Elementos",
                                    delay: 5000
                                });
                                $scope.isLoadingMore = false;
                            }
                        });
                }                    
			}
			
			// User specifics
			$scope.boxHeaderTitleActive = "Usuarios activos";
			$scope.boxHeaderTitleDeactive = "Usuarios de baja";

			$scope.panels = [
				{
					index: 0,
					name: 'Usuarios',
					state: true,
					size: 12
				},
				{
					index: 1,
					//name: 'Usuario: ' + $scope.panelInfoName,
					state: false,
					size: 6
				}
			];

			$scope.panels[1].name = 'Seleccionar usuario';

			$scope.avatarIMG = "/Content/img/usermale.jpg";

			$scope.SexoChange = function () {
				if (($scope.selectedUser.User.Photo === null) || ($scope.selectedUser.User.Photo === undefined)) {
					if ($scope.selectedUser.User.Gender === true) {
						$scope.avatarIMG = "/Content/img/female.jpg";
					} else {
						$scope.avatarIMG = "/Content/img/usermale.jpg";
					}
				}
			};

			// Date filters
			$scope.initDateFilter = false;
			$scope.dateFilter = function () {
				$scope.initDateFilter = true;
				$scope.RefreshList();
				$scope.initDateFilter = false;
			};
			$scope.dateFilterRemove = function () {
				$scope.initDateFilter = true;
				$scope.untilDate = "";
				$scope.fromDate = "";
				$scope.RefreshList();
				$scope.initDateFilter = false;
			};

			$scope.initDateFilterMore = false;
			$scope.dateFilterMore = function () {
				$scope.initDateFilterMore = true;
				$scope.RefreshListMore();
				$scope.initDateFilterMore = false;
			};
			$scope.dateFilterRemoveMore = function () {
				$scope.initDateFilterMore = true;
				$scope.untilDateMore = "";
				$scope.fromDateMore = "";
				$scope.RefreshListMore();
				$scope.initDateFilterMore = false;
			};
		}
	])

;