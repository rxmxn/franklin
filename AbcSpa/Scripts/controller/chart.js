"use strict";
angular.module("abcadmin")
    .controller("ChartController", [
        "$scope", '$controller', "$http", '$rootScope', 'Notification', 'SecurityFact', "mychartsFactory", "$document",
        function ($scope, $controller, $http, $rootScope, notification, securityFact, mychartsFactory, $document) {
            //console.log("AdminHomeController");

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            $scope.employee = securityFact.employeeDataExpress();
			
            function sampletechHome() {
                $scope.panels = [
                    {
                        index: 1,
                        name: 'Reconocimientos por Método',
                        state: true,
                        size: 6
                    },
                    {
                        index: 2,
                        name: 'Métodos reconocidos por Institución',
                        state: true,
                        size: 6
                    },
                    {
                        index: 3,
                        name: 'Signatarios por Institución',
                        state: true,
                        size: 6
                    },
                    {
                        index: 4,
                        name: 'Métodos reconocidos por Región',
                        state: true,
                        size: 6
                    }
                ];

                $scope.address = 'sampletech';
                
                // Plot of TotalMethodsAcknowledged                
                function FillChart1a (dataIndex, toolText)
                {
                    var name = $scope.datasource1.data[dataIndex].label;
                    mychartsFactory.pieChart('/chart/MethodsAckPerEnterprise',
                        { methodName: name },
                        "Institución: $label, Parámetros Reconocidos con el Método " + name +": $datavalue",
                        "Parámetros reconocidos por Institución (Método: " + name + ")")
                    .then(function (response) {
                        if (response.success) {
                            $scope.datasource1a.chart = response.chart;
                            $scope.datasource1a.data = response.data;
                        }
                    });

                    $scope.selectedName1 = toolText;
                    $scope.info1 = $scope.datasource1.data[dataIndex].info;
                }
            	
                $scope.datasource1 = {}
            	mychartsFactory.pieChart('/chart/TotalMethodsAcknowledged', {},
					"Método: $label, Reconocimientos: $datavalue",
					"Reconocimientos por método")
                .then(function (response) {
                	if (response.success) {
                		$scope.datasource1.chart = response.chart;
                		$scope.datasource1.data = response.data;

                		if ($scope.datasource1.data[0] !== undefined) {
                			FillChart1a(0, "Método: " + $scope.datasource1.data[0].label +
                                        ", Reconocimientos: " + $scope.datasource1.data[0].value);
		                }
                	}
                });

                $scope.selectedName1 = "";
                $scope.info1 = [];
	            $scope.datasource1a = {};
                $scope.events1 = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			//$document.context.getElementById('chart1').className = "col-lg-6";
                            FillChart1a(props.dataIndex, props.toolText);                			
                		});
                	}
                }
                
                $scope.selectedName1a = "";
                $scope.info1a = [];
                $scope.events1a = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			$scope.selectedName1a = props.toolText;
                			$scope.info1a = $scope.datasource1a.data[props.dataIndex].info;
                		});
                	}
                }
				
            	// Plot of MethodsAckPerEnterprise 
                function FillChart2a (dataIndex, toolText)
                {
                    var name = $scope.datasource2.data[dataIndex].label;
                    mychartsFactory.pieChart('/chart/MethodAckPerRegion',
                        { ackName: name },
                        "Región: $label, Métodos Reconocidos por " + name + ": $datavalue",
                        "Métodos Reconocidos por Región (Institución: " + name + ")")
                    .then(function (response) {
                        if (response.success) {
                            $scope.datasource2a.chart = response.chart;
                            $scope.datasource2a.data = response.data;
                        }
                    });

                    $scope.selectedName2 = toolText;
                    $scope.info2 = $scope.datasource2.data[dataIndex].info;
                }
                
                $scope.datasource2 = {}
            	mychartsFactory.areaChart('/chart/MethodsAckPerEnterprise', {},
					"Instituciones", "Métodos Reconocidos", "Institución: $label, Métodos Reconocidos: $datavalue",
					"Métodos Reconocidos por Instituciones")
                .then(function (response) {
                	if (response.success) {
                		$scope.datasource2.chart = response.chart;
                		$scope.datasource2.data = response.data;

                		if ($scope.datasource2.data[0] !== undefined) {
                			FillChart2a(0, "Institución: " + $scope.datasource2.data[0].label +
                                        ", Métodos Reconocidos: " + $scope.datasource2.data[0].value);
		                }
                	}
                });

                $scope.selectedName2 = "";
                $scope.info2 = [];
                $scope.datasource2a = {};
                $scope.events2 = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			//$document.context.getElementById('chart2').className = "col-lg-6";
                            FillChart2a(props.dataIndex, props.toolText); 
                		});
                	}
                }

                $scope.selectedName2a = "";
                $scope.info2a = [];
                $scope.events2a = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			$scope.selectedName2a = props.toolText;
                			$scope.info2a = $scope.datasource2a.data[props.dataIndex].info;
                		});
                	}
                }
				
            	// Plot of SignatariosPerEnterprise SignatariosPerRegion SignatariosPerEnterprise
                function FillChart3a (dataIndex, toolText)
                {
                    var name = $scope.datasource3.data[dataIndex].label;
                        mychartsFactory.pieChart('/chart/SignatariosPerRegion',
                            { ackName: name }, "Región: $label, Signatarios: $datavalue (Institución: " + name + ")",
                            "Signatarios por Región (Institución: " + name + ")")
                        .then(function (response) {
                            if (response.success) {
                                $scope.datasource3a.chart = response.chart;
                                $scope.datasource3a.data = response.data;
                            }
                        });

                    $scope.selectedName3 = toolText;
                    $scope.info3 = $scope.datasource3.data[dataIndex].info;
                }
                
                $scope.datasource3 = {}
            	mychartsFactory.areaChart('/chart/SignatariosPerEnterprise', {}, "Instituciones", "Signatarios",
					"Institución: $label, Signatarios: $datavalue", "Signatarios por Instituciones")
                .then(function (response) {
                	if (response.success) {
                		$scope.datasource3.chart = response.chart;
                		$scope.datasource3.data = response.data;

                		if ($scope.datasource3.data[0] !== undefined) {
                			FillChart3a(0, "Institución: " + $scope.datasource3.data[0].label +
                                        ", Signatarios: " + $scope.datasource3.data[0].value);
		                }
                	}
                });

                $scope.selectedName3 = "";
                $scope.info3 = [];
                $scope.datasource3a = {};
                $scope.events3 = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			//$document.context.getElementById('chart3').className = "col-lg-6";
                            FillChart3a(props.dataIndex, props.toolText); 
                		});
                	}
                }

                $scope.selectedName3a = "";
                $scope.info3a = [];
                $scope.events3a = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			$scope.selectedName3a = props.toolText;
                			$scope.info3a = $scope.datasource3a.data[props.dataIndex].info;
                		});
                	}
                }

            	// Pie of MethodAckPerRegion
                function FillChart4a (dataIndex, toolText)
                {
                    var name = $scope.datasource4.data[dataIndex].label;
                    mychartsFactory.pieChart('/chart/TotalMethodsAcknowledged',
                        { regionName: name }, "Método: $label, Reconocimientos: $datavalue (Región: " + name + ")",
                        "Reconocimientos por Método (Región: " + name + ")")
                    .then(function (response) {
                        if (response.success) {
                            $scope.datasource4a.chart = response.chart;
                            $scope.datasource4a.data = response.data;
                        }
                    });

                    $scope.selectedName4 = toolText;
                    $scope.info4 = $scope.datasource4.data[dataIndex].info;
                }
                
                $scope.datasource4 = {}
            	mychartsFactory.pieChart('/chart/MethodAckPerRegion', {}, "Región: $label, " +
			            "Métodos Acreditados: $datavalue", "Métodos reconocidos por región")
                .then(function (response) {
                	if (response.success) {
                		$scope.datasource4.chart = response.chart;
                		$scope.datasource4.data = response.data;

                		if ($scope.datasource4.data[0] !== undefined) {
                			FillChart4a(0, "Método: " + $scope.datasource4.data[0].label +
                                    ", Reconocimientos: " + $scope.datasource4.data[0].value);
		                }
                	}
                });

                $scope.selectedName4 = "";
                $scope.info4 = [];
	            $scope.datasource4a = {};
                $scope.events4 = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			//$document.context.getElementById('chart4').className = "col-lg-6";
                            FillChart4a(props.dataIndex, props.toolText);                 			
                		});
                	}
                }

                $scope.selectedName4a = "";
                $scope.info4a = [];
                $scope.events4a = {
                	dataplotclick: function (ev, props) {
                		$scope.$apply(function () {
                			$scope.selectedName4a = props.toolText;
                			$scope.info4a = $scope.datasource4a.data[props.dataIndex].info;
                		});
                	}
                }
            }
            
            if ($scope.employee !== null) {
            	var locationCase = $scope.employee.UserName === 'root' ? "Administrador" : $scope.employee.Role.Name;
            	switch (locationCase) {
            		case "Administrador": sampletechHome(); break;
            		case "Técnico Operativo Muestreo": sampletechHome(); break;
            		default: break;
            	}
	        }
        }
    ])
    
.directive("minimizeHomeDirective", ['$document', function ($document) {
    return {
        link: function ($scope, elem, attrs) {

            $scope.minimize = function (element) {
                element.state = !element.state;
                
                var panel = [
                    $document.context.getElementById('panel1'),
                    $document.context.getElementById('panel2'),
                    $document.context.getElementById('panel3'),
                    $document.context.getElementById('panel4')
                ];

                var panelsContent = [
                    $document.context.getElementById('panel1Content'),
                    $document.context.getElementById('panel2Content'),
                    $document.context.getElementById('panel3Content'),
                    $document.context.getElementById('panel4Content')
                ];

                var opened = [], openedContent = [];
                for (var i = 0; i < 4; i++) {
                    if ($scope.panels[i].state) {
                        opened.push(panel[i]);
                        openedContent.push(panelsContent[i]);
                    }
                }

                switch (opened.length) {
                    case 1:
                        opened[0].style.height = "100%";
                        openedContent[0].style.height = "93%";
                        openedContent[0].parentNode.parentNode.className = "col-lg-12";
                        break;
                    case 2:
                    	opened[0].style.height = "100%";
                    	opened[1].style.height = "100%";
                        openedContent[0].style.height = "93%";
                        openedContent[1].style.height = "93%";
                        openedContent[0].parentNode.parentNode.className = "col-lg-6";
                        openedContent[1].parentNode.parentNode.className = "col-lg-6";
                        break;
                    case 3:
                    	opened[0].style.height = "50%";
                        opened[1].style.height = "50%";
                        opened[2].style.height = "50%";
                        openedContent[0].style.height = "88%";
                        openedContent[1].style.height = "88%";
                        openedContent[2].style.height = "88%";
                        break;
                    case 4:
                    	opened[0].style.height = "50%";
                    	opened[1].style.height = "50%";
                    	opened[2].style.height = "50%";
                    	opened[3].style.height = "50%";
                        openedContent[0].style.height = "88%";
                        openedContent[1].style.height = "88%";
                        openedContent[2].style.height = "88%";
                        openedContent[3].style.height = "88%";
                        break;
                }
            }

	        $scope.allElemntsState = true;

            $scope.full = function(element) {
            	for (var i = 0; i < $scope.panels.length; i++) {
            		if ($scope.panels[i].index !== element.index) {
            			$scope.panels[i].state = $scope.allElemntsState;
            			$scope.minimize($scope.panels[i]);
		            }
            	}
            	$scope.allElemntsState = !$scope.allElemntsState;
            }
        }
    }
}])

;