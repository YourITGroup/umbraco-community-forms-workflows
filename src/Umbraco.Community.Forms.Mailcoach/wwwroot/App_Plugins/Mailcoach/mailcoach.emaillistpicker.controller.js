function mailingListPickerController($scope, $http) {
  var vm = this
  vm.lists = []
  vm.error = null
  vm.loading = true

  function loadLists() {
    vm.loading = true
    vm.error = null

    $http
      .get("/umbraco/backoffice/Mailcoach/MailcoachApi/GetMailingLists")
      .then(function (response) {
        vm.lists = response.data
        vm.loading = false
        vm.selectedList = $scope.setting.value

        if (vm.lists.length === 0) {
          vm.error =
            "No email lists found. Please check your Mailcoach configuration."
        }
      })
      .catch(function (error) {
        vm.loading = false
        vm.error =
          "Failed to load email lists: " +
          (error.data?.error || error.statusText || "Unknown error")
        console.error("Mailcoach API error:", error)
      })
  }

  // Load email lists on initialization
  loadLists()

  vm.save = function () {
    $scope.setting.value = vm.selectedList.length > 0 ? vm.selectedList : ""
  }
}

angular
  .module("umbraco")
  .controller(
    "Umbraco.Community.Forms.Mailcoach.EmailListPickerController",
    mailingListPickerController
  )
