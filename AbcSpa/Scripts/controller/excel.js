angular.module('abcadmin')
    .controller('ExcelController',
    [
        '$scope', '$route', '$location', '$http', '$cookies', '$controller', 'SecurityFact', 'Notification', 'ngDialog', 'ExcelUploadService',
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact, notification, ngDialog, excelUploadService) {
            //Codigo a usar en todos los controladores.
            $controller('RootController', { $scope: $scope });
            $scope.employee = securityFact.employeeDataExpress();
            $scope.loadAvatar();
            //Fin del codigo a utilizar en todos los controladores

            var normalDelay = 3000;

            $scope.sectionTitle = "EXCEL DUTY";

            $scope.orderByField = "Title";
            $scope.reverseSort = false;

            $scope.isLoading = false;
            //write all code for the pagination
            $scope.pageInit = 0;
            $scope.pageEnd = 10;
            $scope.pageLabels = [];

            //function loadExcel() {
            //    $scope.isLoading = true;
            //    $scope.pageLabels = [];
            //    $http.post("/procedure/getprocedurelist", {

            //    })
            //        .success(function (response) {
            //            if (response.success == true) {
            //                //$scope.procedimientoList = response.procedures;
            //                $scope.countPage = $scope.procedimientoList.length / 10;
            //                if ($scope.countPage != 0) {
            //                    for (var i = 0; i < $scope.countPage; i++)
            //                        $scope.pageLabels.push({ label: i + 1, active: "" });
            //                    $scope.pageLabels[$scope.pageInit].active = "active";
            //                }
            //                $scope.isLoading = false;
            //            } else {
            //                Notification.error({
            //                    message: "Error al recuperar los datos.", delay: errorDelay
            //                });
            //                $scope.isLoading = false;
            //            }
            //        })
            //        .error(function () {
            //            Notification.error({
            //                message: "Error en la conexión con el servidor.", delay: errorDelay
            //            });
            //            $scope.isLoading = false;
            //        });
            //}

            //loadExcel();


            //-------------------------------------------------------
            function checkFileValid(file) {
                if (file.type !== 'application/vnd.ms-excel' && file.type !== 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
                    notification.error({ message: "Error: Solo se admiten archivos Excel .xls o .xlsx", delay: 3000 });
                    return false;
                }

                if (file.size > 29360128) {
                    notification.error({ message: "Error: No se admiten archivos mayores de 28MB.", delay: 3000 });
                    return false;
                }

                return true;
            }

//-------------------------------------------------------
            function cleanSelection() {
                angular.forEach(angular.element("input[type='file']"), function(inputElem) {
                    angular.element(inputElem).val(null);
                });
            }

//-------------------------------------------------------
            $scope.cleanProcedDoc = function() {
                //var nombre = $scope.newEmployee.Photo.split('/');
                //console.log($scope.newProcedure.Document);
                //$http.post('/usuario/removeimage', {name: nombre[nombre.length-1]});


//$scope.newProcedure.Document = null;
            }; //-------------------------------------------------------
            $scope.selecExcelforUpload = function(file) {
                $scope.cleanProcedDoc();
                var fileSelected = file[0];
                if (checkFileValid(fileSelected)) {
                    excelUploadService.UploadFile(fileSelected)
                        .then(function(d) {
                            //$scope.newProcedure.Document = "/Content/uploads/" + d.codeName;
                            //$scope.newProcedure.RealDocumentTitle = d.realName;
                            $scope.file_path = "Documento: " + d.realName;
                            cleanSelection();
                            notification.success({
                                message: "Documento subido con éxito.",
                                delay: normalDelay
                            });
                        }, function(e) {
                            notification.error({ message: e, delay: 3000 });
                        });
                } else {
                    $scope.Message = "All the fields are required.";
                }
            }; //------------------------------------------------
            function cleanSelectedPage() {
                for (var i in $scope.pageLabels) {
                    $scope.pageLabels[i].active = "";
                }
            }

            $scope.showPage = function(page) {
                cleanSelectedPage();
                page.active = "active";
                $scope.pageEnd = page.label * 10;
                $scope.pageInit = $scope.pageEnd - 10;
            };
            $scope.nextPage = function() {
                for (var i = 0; i < $scope.pageLabels.length; i++) {
                    if ($scope.pageLabels[i].active == "active") {
                        $scope.showPage($scope.pageLabels[i + 1]);
                        i = $scope.pageLabels.length;
                    }
                }
            };
            $scope.prevPage = function() {
                for (var i in $scope.pageLabels) {
                    if ($scope.pageLabels[i].active == "active") {
                        $scope.showPage($scope.pageLabels[i - 1]);
                    }
                }
            }; //------------------------------------


            $scope.myDataSource = {
                chart: {
                    caption: "Harry's SuperMart",
                    subCaption: "Top 5 stores in last month by revenue",
                    numberPrefix: "$",
                    theme: "fint"
                },
                data: [{
                    label: "Bakersfield Central",
                    value: "880000"
                }, {
                    label: "Garden Groove harbour",
                    value: "730000"
                }, {
                    label: "Los Angeles Topanga",
                    value: "590000"
                }, {
                    label: "Compton-Rancho Dom",
                    value: "520000"
                }, {
                    label: "Daly City Serramonte",
                    value: "330000"
                }]
            };


            $scope.bubleDataSource = {
                chart: {
                    caption: "Sales Analysis of Shoe Brands",
                    subcaption: "Last Quarter",
                    xAxisMinValue: "0",
                    xAxisMaxValue: "100",
                    yAxisMinValue: "0",
                    yAxisMaxValue: "30000",
                    plotFillAlpha: "70",
                    plotFillHoverColor: "#6baa01",
                    showPlotBorder: "0",
                    xAxisName: "Average Price",
                    yAxisName: "Units Sold",
                    numDivlines: "2",
                    showValues: "1",
                    showTrendlineLabels: "0",
                    plotTooltext: "$name : Profit Contribution - $zvalue%",
                    drawQuadrant: "1",
                    quadrantLineAlpha: "80",
                    quadrantLineThickness: "3",
                    quadrantXVal: "50",
                    quadrantYVal: "15000",
                    quadrantLabelTL: "Low Price / High Sale",
                    quadrantLabelTR: "High Price / High Sale",
                    quadrantLabelBL: "Low Price / Low Sale",
                    quadrantLabelBR: "High Price / Low Sale",
                    theme: "fint"
                },
                categories: [
                    {
                        category: [
                            {
                                label: "$0",
                                x: "0"
                            },
                            {
                                label: "$20",
                                x: "20",
                                showverticalline: "1"
                            },
                            {
                                label: "$40",
                                x: "40",
                                showverticalline: "1"
                            },
                            {
                                label: "$60",
                                x: "60",
                                showverticalline: "1"
                            },
                            {
                                label: "$80",
                                x: "80",
                                showverticalline: "1"
                            },
                            {
                                label: "$100",
                                x: "100",
                                showverticalline: "1"
                            }
                        ]
                    }
                ],
                dataset: [
                    {
                        color: "#00aee4",
                        data: [
                            {
                                x: "80",
                                y: "15000",
                                z: "24",
                                name: "Nike"
                            },
                            {
                                x: "60",
                                y: "18500",
                                z: "26",
                                name: "Adidas"
                            },
                            {
                                x: "50",
                                y: "19450",
                                z: "19",
                                name: "Puma"
                            },
                            {
                                x: "65",
                                y: "10500",
                                z: "8",
                                name: "Fila"
                            },
                            {
                                x: "43",
                                y: "8750",
                                z: "5",
                                name: "Lotto"
                            },
                            {
                                x: "32",
                                y: "22000",
                                z: "10",
                                name: "Reebok"
                            },
                            {
                                x: "44",
                                y: "13000",
                                z: "9",
                                name: "Woodland"
                            }
                        ]
                    }
                ],
                trendlines: [
                    {
                        line: [
                            {
                                startValue: "20000",
                                endValue: "30000",
                                isTrendZone: "1",
                                color: "#aaaaaa",
                                alpha: "14"
                            },
                            {
                                startValue: "10000",
                                endValue: "20000",
                                isTrendZone: "1",
                                color: "#aaaaaa",
                                alpha: "7"
                            }
                        ]
                    }
                ],
                vTrendlines: [
                    {
                        line: [
                            {
                                startValue: "44",
                                isTrendZone: "0",
                                color: "#0066cc",
                                thickness: "1",
                                dashed: "1",
                                displayValue: "Gross Avg."
                            }
                        ]
                    }
                ]
            };












            $scope.attrs = {

                caption: "Sales Comparison: 2013 versus 2014",
                subCaption: "Harry’ s SuperMart",
            numberprefix: "$",
            plotgradientcolor: "",
            bgcolor: "FFFFFF",
            showalternatehgridcolor: "0",
            divlinecolor: "CCCCCC",
            showvalues: "0",
            showcanvasborder:"0",
            canvasborderalpha: "0",
            canvasbordercolor: "CCCCCC",
            canvasborderthickness: "1",
            yaxismaxvalue: "30000",
            captionpadding: "30",
            linethickness: "3",
            yaxisvaluespadding: "15",
            legendshadow: "0",
            legendborderalpha: "0",
            palettecolors: "#f8bd19,#008ee4,#33bdda,#e44a00,#6baa01,#583e78",
            showborder: "0"
        };

        $scope.categories = [{
            "category": [{
                label: "Jan"
            }, {
                label: "Feb"
            }, {
                label: "Mar"
            }, {
                label: "Apr"
            }, {
                label: "May"
            }, {
                label: "Jun"
            }, {
                label: "Jul"
            }, {
                label: "Aug"
            }, {
                label: "Sep"
            }, {
                label: "Oct"
            }, {
                label: "Nov"
            }, {
                label: "Dec"
            }]
        }];

        $scope.dataset = [{
            "seriesname": "2013",
            "data": [{
                value: "22400"
            }, {
                value: "24800"
            }, {
                value: "21800"
            }, {
                value: "21800"
            }, {
                value: "24600"
            }, {
                value: "27600"
            }, {
                value: "26800"
            }, {
                value: "27700"
            }, {
                value: "23700"
            }, {
                value: "25900"
            }, {
                value: "26800"
            }, {
                value: "24800"
            }]
        },

            {
                "seriesname": "2012",
                "data": [{
                    value: "10000"
                }, {
                    value: "11500"
                }, {
                    value: "12500"
                }, {
                    value: "15000"
                }, {
                    value: "16000"
                }, {
                    value: "17600"
                }, {
                    value: "18800"
                }, {
                    value: "19700"
                }, {
                    value: "21700"
                }, {
                    value: "21900"
                }, {
                    value: "22900"
                }, {
                    value: "20800"
                }]
            }
        ];






        $scope.dataSource = {
            chart: {
                caption: "Quarterly Revenue for FY2013-2014",
                subCaption: "Harry's SuperMart",
                xAxisName: "Quarter",
                yAxisName: "Revenue",
                numberPrefix: "$",
                showValues: "0",
                theme: "fint"
            },
            data: [{
                label: "Q1",
                value: "420000"
            }, {
                label: "Q2",
                value: "810000"
            }, {
                label: "Q3",
                value: "720000"
            }, {
                label: "Q4",
                value: "550000"
            }]
        };
            // The `selectedValue` variable is assigned an initial value that will be displayed when the chart is
            // first rendered.
        $scope.selectedValue = "nothing";
            // When the `dataplotclick` event is triggered, the `$apply` scope object is used to execute the
            // function that checks for the value of the data plot clicked.
            //This value is assigned to the `selectedValue` variable.
            // Consequently, the text rendered below the chart is updated.
            $scope.events = {
                dataplotclick: function(ev, props) {
                    $scope.$apply(function() {
                        $scope.selectedValue = props.displayValue;
                    });
                }
            };


            $scope.pieDataSource = {
                chart: {
                    caption: "Age profile of website visitors",
                    subcaption: "Last Year",
                    startingangle: "120",
                    showlabels: "0",
                    showlegend: "1",
                    enablemultislicing: "0",
                    slicingdistance: "15",
                    showpercentvalues: "1",
                    showpercentintooltip: "0",
                    plottooltext: "Age group : $label Total visit : $datavalue",
                    theme: "fint"
                },
                data: [
                    {
                        label: "Teenage",
                        value: "1250400"
                    },
                    {
                        label: "Adult",
                        value: "1463300"
                    },
                    {
                        label: "Mid-age",
                        value: "1050700"
                    },
                    {
                        label: "Senior",
                        value: "491000"
                    }
                ]
            };


            $scope.gaugeDataSource = {
                chart: {
                    caption: "Customer Satisfaction Score",
                    subcaption: "Last week",
                    lowerLimit: "0",
                    upperLimit: "100",
                    lowerLimitDisplay: "Bad",
                    upperLimitDisplay: "Good",
                    showValue: "1",
                    valueBelowPivot: "1",
                    theme: "fint"
                },
                colorRange: {
                    color: [
                        {
                            minValue: "0",
                            maxValue: "50",
                            code: "#e44a00"
                        },
                        {
                            minValue: "50",
                            maxValue: "75",
                            code: "#f8bd19"
                        },
                        {
                            minValue: "75",
                            maxValue: "100",
                            code: "#6baa01"
                        }
                    ]
                },
                dials: {
                    dial: [
                        {
                            value: "67"
                        }
                    ]
                }
            };


        }
    ])

.factory("ExcelUploadService", function ($http, $q) {
    var fac = {};
    fac.UploadFile = function (file) {
        var formData = new FormData();
        formData.append("attachment", file);

        var defer = $q.defer();
        $http.post("/excel/uploadexcel", formData, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        })
        .success(function (d) {
            defer.resolve(d);
        })
        .error(function () {
            defer.reject("File Upload Failed!");
        });

        return defer.promise;
    }
    return fac;
})

;