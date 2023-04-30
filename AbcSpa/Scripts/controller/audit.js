"use strict";

angular.module("abcadmin")
	.controller("AuditController",
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
			//$scope.viewDeactivated = false; // nunca se modifica pq los audit no se desactivan

			var filters = function(page) {
				return {
					page: (page === null || page === undefined) ? $scope.currentPage : page,
					pageSize: $scope.pageSize,
					searchGeneral: $scope.searchGeneral,
					fromDate: $scope.fromDate === undefined ? null : $scope.fromDate,
					untilDate: $scope.untilDate === undefined ? null : $scope.untilDate,
					searchIp: $scope.searchIp
				}
			};
			
			$scope.elementsList = null;
			$scope.isLoading = false;

			$scope.RefreshList = function(page) {
				$scope.isLoading = true;
				generalFactory.RefreshList("/audit/RefreshAudit", filters, page)
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

			$scope.selectedTable = null;

			$scope.more = function(table) {
				$scope.selectedTable = table;
				$scope.panels[1].name = $scope.selectedTable.TableName;
				$scope.panels[1].state = false;
				$scope.minimize($scope.panels[1]);
				$scope.RefreshListMore(1);
			}
			
			$scope.currentPageMore = 1;
			$scope.pageSizeMore = 10;

			$scope.searchGeneralMore = "";
			$scope.searchIpMore = "";
			$scope.viewAdded = true;
			$scope.viewModified = true;

			var filtersMore = function (page) {
				return {
					page: (page === null || page === undefined) ? $scope.currentPageMore : page,
					pageSize: $scope.pageSizeMore,
					tableName: $scope.selectedTable === undefined ? null : $scope.selectedTable.TableName,
					fromDate: $scope.fromDateMore === undefined ? null : $scope.fromDateMore,
					untilDate: $scope.untilDateMore === undefined ? null : $scope.untilDateMore,
					searchGeneral: $scope.searchGeneralMore,
					searchIp: $scope.searchIpMore,
					viewAdded: $scope.viewAdded,
					viewModified: $scope.viewModified
				}
			};

			$scope.elementsListMore = null;
			$scope.isLoadingMore = false;

			$scope.RefreshListMore = function (page) {
                if($scope.selectedTable !== null)
                {
                    $scope.isLoadingMore = true;
				    generalFactory.RefreshList("/audit/RefreshAuditInfo", filtersMore, page)
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
			$scope.boxHeaderTitleActive = "Tablas modificadas";

			$scope.panels = [
				{
					index: 0,
					name: 'Tablas',
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

			$scope.panels[1].name = 'Seleccionar tabla';
			
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