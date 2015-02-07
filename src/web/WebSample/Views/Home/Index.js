+function () {
    var vm = avalon.define("account", function (vm) {
        vm.args = { pageNumber: 1, pageSize: 5, likeRealName: '', likeUsername: '' };
        vm.users = { rows: [], total: 0 };
        vm.current = {};
        vm.displayCurrent = false;

        vm.search = function (e) {
            if (e) e.preventDefault();
            var r = aoite.remote("account/findall", vm.args);
            if (test(r)) vm.users = r.value;
        }
        vm.calcTotalPage = function () {
            if (vm.users && vm.users.total > 0) {
                var totalPage = vm.users.total / vm.args.pageSize;
                if (parseInt(totalPage) != totalPage) totalPage += 1;
                return parseInt(totalPage);
            }
            return 1;
        };
        vm.nextPage = function (page) {
            var p = vm.args.pageNumber;
            p += page;
            return vm.goPage(p);
        };
        vm.goPage = function (page) {
            if (page < 1 || page > vm.calcTotalPage()) return false;
            vm.args.pageNumber = page;
            vm.search();
            return true;
        };

        vm.remove = function (user) {
            if (user.username == 'admin') {
                alert('不能删除管理员账号！')
                return;
            }
            if (test(aoite.remote("account/remove", [user.id]))) {
                vm.users.rows.remove(user);
                vm.users.total -= 1;
                if (!vm.users.rows.length) reloadNow();
            }
        };

        vm.showCurrent = function (user) {
            vm.current = user;
            vm.displayCurrent = true;
        };
        vm.hideCurrent = function () {
            vm.displayCurrent = false;
            vm.current = {};
        };
        vm.saveCurrent = function (e) {
            e.preventDefault();
            var r = aoite.remote("account/save", vm.current);
            if (test(r)) {
                if (!vm.current.id) {
                    vm.current.id = r.value;
                    if (vm.users.rows.length < vm.args.pageSize)
                        vm.users.rows.push(vm.current);
                    vm.users.total += 1;
                }
                vm.hideCurrent();
            }
        };
    });
    vm.search();
}();