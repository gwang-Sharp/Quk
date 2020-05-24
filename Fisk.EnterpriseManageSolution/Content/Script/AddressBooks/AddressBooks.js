
var vm = new Vue({
    el: "#container",
    data: {
        myDeparts: [],
        allDeparts: [],
        MYmembers: [],
        ChooseDepartID: 0,
        IDarray: [],
        hasMember: [],
        List: []
    },
    mounted: function () {
        this.allDeparts = JSON.parse($("#DataStore").val());
        this.myDeparts = this.allDeparts.filter(a => {
            return a.parentid == 1;
        });
        this.hasMember = JSON.parse(sessionStorage.getItem("member"));
    },
    methods: {
        GetDepart: function (id) {
            vm.IDarray.push(id);
            console.log(vm.IDarray);
            vm.ChooseDepartID = id;
            vm.myDeparts = vm.allDeparts.filter(a => {
                return a.parentid == id;
            });
            //console.log(vm.myDeparts)
            if (vm.myDeparts.length == 0) {
                $.ajax({
                    url: "/main/getDepartMember",
                    type: "post",
                    dataType: "json",
                    data: {
                        departID: id
                    },
                    beforeSend: function () {
                        loading = vm.$loading({
                            text: "正在拼命加载....."
                        });
                    },
                    success: function (data) {
                        loading.close();
                        //console.log(JSON.parse(data.members));
                        var pmid = data.PmID;
                        vm.MYmembers = JSON.parse(data.members).filter(m => {
                            return m.userid != pmid;
                        });

                        vm.MYmembers = JSON.parse(data.members).filter(m => {
                            return m.position.toString().indexOf("经理") == -1;
                        });
                        //console.log(vm.MYmembers);
                        if (vm.MYmembers.length > 0) {
                            $("#depart").toggle();
                            $("#members").toggle();
                            $("#addLog").toggle();
                        }
                    }
                });
            }
        },
        FilterMember: function (obj) {
            var m = JSON.stringify(obj);
            m = JSON.parse(m);
            m.DepartmentID = vm.ChooseDepartID;
            m.isleader = "false";
            var has = this.hasMember.find(h => {
                return h.name == m.name;
            });
            if (has) {
                this.List.push(has);
                this.hasMember = this.hasMember.filter(h => {
                    return h.name != has.name;
                })
            }
            else {
                if (this.List.length > 0) {
                    var before = this.List.find(l => {
                        return l.name == m.name;
                    });
                    m.isleader = before.Isprincipal.toString();
                    this.List = this.List.filter(l => {
                        return l.name != m.name;
                    });
                }
                this.hasMember.push(m);
            }
        },
        ChooseMember: function () {
            let that = this;
            $.actions({
                actions: [{
                    text: "确认",
                    onClick: function () {
                        $.each(that.hasMember, function (index, value) {
                            if (!value.isleader) {
                                value.isleader = value.Isprincipal.toString();
                            }
                        })
                        $.ajax({
                            url: "/main/ProTeamAdd",
                            type: "post",
                            data: {
                                members: JSON.stringify(that.hasMember)
                            },
                            dataType: "json",
                            beforeSend: function () {
                                loading = vm.$loading({
                                    text: "正在拼命提交....."
                                });
                            },
                            success: function (data) {
                                loading.close();
                                that.List = [];
                                sessionStorage["member"] = JSON.stringify(that.hasMember);
                                $.toast.prototype.defaults.duration = 1000;
                                $.toast(data.msg, function () {
                                    window.location.href = "/Main/ProjectDetail";
                                });
                            }
                        });
                    }
                }]
            });
        },
        goback: function () {
            if (vm.IDarray.length == 0) {
                window.location.href = "/Main/ProjectDetail";
            }
            else if (vm.IDarray.length == 1) {
                vm.myDeparts = vm.allDeparts.filter(a => {
                    return a.parentid == 1;
                });
                if (vm.IDarray[0] != 3) {
                    vm.MYmembers = [];
                    $("#depart").toggle();
                    $("#members").toggle();
                    $("#addLog").toggle();
                }
                vm.IDarray = [];
            }
            else if (vm.IDarray.length == 2) {
                vm.myDeparts = vm.allDeparts.filter(a => {
                    return a.parentid == vm.IDarray[0];
                });
                vm.IDarray.pop();
                if (vm.MYmembers.length > 0) {
                    vm.MYmembers = [];
                    $("#depart").toggle();
                    $("#members").toggle();
                    $("#addLog").toggle();
                }
            }
            else {
                vm.MYmembers = [];
                vm.myDeparts = vm.allDeparts.filter(a => {
                    return a.parentid == vm.IDarray[1];
                });
                vm.IDarray.pop();
                $("#depart").toggle();
                $("#members").toggle();
                $("#addLog").toggle();
            }
        }
    }
});