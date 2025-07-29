angular.module("umbraco").controller("MailcoachEmailListPickerController",
    function ($scope, $http) {
        
        $scope.loading = true;
        $scope.error = null;
        $scope.emailLists = [];

        // Initialize the model if not set
        if (!$scope.model) {
            $scope.model = { value: "" };
        }

        // Load email lists from Mailcoach API
        function loadEmailLists() {
            $scope.loading = true;
            $scope.error = null;

            $http.get("/umbraco/backoffice/Mailcoach/MailcoachApi/GetEmailLists")
                .then(function (response) {
                    $scope.emailLists = response.data;
                    $scope.loading = false;
                    
                    if ($scope.emailLists.length === 0) {
                        $scope.error = "No email lists found. Please check your Mailcoach configuration.";
                    }
                })
                .catch(function (error) {
                    $scope.loading = false;
                    $scope.error = "Failed to load email lists: " + (error.data?.error || error.statusText || "Unknown error");
                    console.error("Mailcoach API error:", error);
                });
        }

        // Load email lists on initialization
        loadEmailLists();
    }
);