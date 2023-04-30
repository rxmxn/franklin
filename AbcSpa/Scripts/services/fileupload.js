"use strict";
angular.module("abcadmin")
	.factory("ImageUploadService", ["$http", "$q", "Notification",
		function ($http, $q, notification) {

		var fac = {};

		fac.UploadFile = function(file) {
			var formData = new FormData();
			formData.append("file", file);

			var defer = $q.defer();
			$http.post("/files/saveimage", formData, {
					withCredentials: true,
					headers: { 'Content-Type': undefined },
					transformRequest: angular.identity
				})
				.success(function(d) {
					defer.resolve(d);
				})
				.error(function() {
					defer.reject("File Upload Failed!");
				});

			return defer.promise;
		};

		fac.CheckFileValid = function(file) {
			if (((file.type === "image/png" || file.type === "image/jpeg" || file.type === "image/jpg") 
				&& file.size <= (512 * 1024)) 
				|| ((file.type === "application/pdf") && file.size <= (5120 * 1024))) {
				return true;
			}
			else {
				notification.error({ message: "Error: Solo se admiten PNG, JPEG/JPG menores que 512 KB o PDF menores de 5 MB", delay: 3000 });
				return false;
			}
		}

		fac.CleanSelection = function() {
			angular.forEach(angular.element("input[type='file']"), function(inputElem) {
				angular.element(inputElem).val(null);
			});
		};

		fac.FileToUpload = function (file) {
			var defer = $q.defer();
			var fileSelected = file[0];
			var fileName = "/Content/uploads/";
			if (fac.CheckFileValid(fileSelected)) {
				fac.UploadFile(fileSelected)
                .then(function (d) {
                	fileName += d.name;
                	fac.CleanSelection();
                	defer.resolve({ success: true, fileName: fileName });
                }, function (e) {
                	defer.resolve({ success: false });
                });
			}
			else {
				defer.resolve({ success: false });
			}
			return defer.promise;
		};

		return fac;
	}]);