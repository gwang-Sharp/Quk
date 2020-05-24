
var WEEKvm = new Vue({
    el: "#WeekReport",
    data: {
        Week: new Date(),
        allProjects: [],
        allWeekDays: 0,
        WeekRePortData: [],
        isUpdate: false
    },
    mounted: function () {
        setTimeout(function () {
            Master.TabbarSelected = 'Personal';
            Master.BottomMenuDisplay = true;
        }, 0);
        var v = JSON.parse($("#DataStore").val());
        this.allProjects = v.Pros;
        var HasWeekReport = v.Edit;
        if (HasWeekReport) {
            this.WeekRePortData = this.allProjects;
            this.isUpdate = true;
        }
    },
    methods: {
        UpdateWeekRecordLog: function () {
            $.toast.prototype.defaults.duration = 800;
            if (!WEEKvm.IsSameWeek()) {
                $.toast("只能对本周进行编辑", "forbidden");
                return;
            }
            $.each(WEEKvm.WeekRePortData, function (inde, obj) {
                for (var i = 0; i < obj.length; i++) {
                    WEEKvm.allWeekDays += parseInt(obj[i].WorkingTime);
                }
            });
            if (WEEKvm.allWeekDays == 0) {
                $.toast("请先选择工时", "cancel");
                return;
            }
            if (WEEKvm.allWeekDays > 7) {
                $.toast("人天总和不得大于七天", "cancel");
                WEEKvm.allWeekDays = 0;
                return;
            }
            var weekDay = WEEKvm.DateUpdate();
            $.ajax({
                url: "/Main/UpdateWeekLOG",
                type: "post",
                dateType: "json",
                data: {
                    week: weekDay,
                    proWeekLog: JSON.stringify(WEEKvm.WeekRePortData)
                },
                beforeSend: function () {
                    loading = WEEKvm.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    $.toast(data.msg, 1000);
                    WEEKvm.allWeekDays = 0;
                    WEEKvm.isUpdate = true;
                }
            });
        },
        BeforeSubmit: function () {
            $.toast.prototype.defaults.duration = 500;
            $.each(WEEKvm.allProjects, function (index, obj) {
                for (var i = 0; i < obj.length; i++) {
                    WEEKvm.allWeekDays += parseInt(obj[i].WorkingTime);
                }
            });
            if (WEEKvm.allWeekDays == 0) {
                $.toast("请先选择工时", "cancel");
                return false;
            }
            if (WEEKvm.allWeekDays > 7) {
                $.toast("人天总和不得大于七天", "cancel");
                WEEKvm.allWeekDays = 0;
                return false;
            }
            var weekDay = WEEKvm.DateUpdate();
            $.each(WEEKvm.allProjects, function (index, obj) {
                for (var a = 0; a < obj.length; a++) {
                    obj[a].NowWeekDay = weekDay;
                }
            });
            return true;
        },
        WeekRecordLogADD: function () {
            if (WEEKvm.isUpdate) {
                WEEKvm.UpdateWeekRecordLog();//修改周报日志
                return;
            }
            if (WEEKvm.BeforeSubmit()) {
                $.ajax({
                    url: "/Main/WeekRecordLog",
                    type: "post",
                    data: { proWeekLog: JSON.stringify(WEEKvm.allProjects) },
                    dataType: "json",
                    beforeSend: function () {
                        loading = WEEKvm.$loading({
                            text: "正在拼命提交....."
                        });
                    },
                    success: function (data) {
                        WEEKvm.WeekRePortData = WEEKvm.allProjects;
                        loading.close();
                        $.toast(data.msg, 1000);
                        WEEKvm.allWeekDays = 0;
                        WEEKvm.isUpdate = true;
                    }
                });
            }
        },
        WorkTimeClear: function () {
            if (WEEKvm.allProjects.length > 0) {
                $.each(WEEKvm.allProjects, function (inde, obj) {
                    for (var i = 0; i < obj.length; i++) {
                        obj[i].WorkingTime = 0;
                    }
                });
            }
            if (WEEKvm.WeekRePortData.length > 0) {
                $.each(WEEKvm.WeekRePortData, function (inde, obj) {
                    for (var i = 0; i < obj.length; i++) {
                        obj[i].WorkingTime = 0;
                    }
                });
            }
            $(".is-dark").hide();
        },
        LoadWeekRecord: function () {
            WEEKvm.allProjects = [];
            var length = WEEKvm.WeekRePortData.length;
            WEEKvm.WeekRePortData.splice(0, length);
            var week = WEEKvm.DateUpdate();
            $.ajax({
                url: "/Main/GetWeekRecord",
                type: "post",
                dataType: "json",
                data: { WeekDay: week },
                beforeSend: function () {
                    loading = WEEKvm.$loading({
                        text: "正在拼命获取....."
                    });
                },
                success: function (data) {
                    loading.close();
                    WEEKvm.isUpdate = true;
                    var weekhid = document.getElementById("week").style.display;
                    WEEKvm.WeekRePortData = data;
                    if (weekhid == "block") {
                        $("#week").toggle();
                        $("#WeekData").toggle();
                    }
                }
            });
        },
        DateUpdate: function () {
            var date = new Date(WEEKvm.Week);
            var dayTime = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            if (date.getDay() != 1) {
                var day = date.getDay();
                if (day - 1 == 1)
                    dayTime = date.getDate() - 1;
                else if (day - 1 == 2)
                    dayTime = date.getDate() - 2;
                else if (day - 1 == 3)
                    dayTime = date.getDate() - 3;
                else if (day - 1 == 4)
                    dayTime = date.getDate() - 4;
                else if (day - 1 == 5)
                    dayTime = date.getDate() - 5;
                else if (day - 1 < 0) {
                    dayTime = date.getDate() - 6;
                }
                if (dayTime < 0) {
                    //跨月
                    var beforeDate = new Date(year, month - 1, dayTime);
                    year = beforeDate.getFullYear();
                    month = beforeDate.getMonth() + 1;
                    dayTime = beforeDate.getDate();
                }
            }
            var datetime = year + '-' + (month < 10 ? "0" + month : month) + '-' + (dayTime < 10 ? "0" + dayTime : dayTime);
            return datetime;
        },
        IsSameWeek: function () {
            var oneDayTime = 1000 * 60 * 60 * 24;
            var old_count = parseInt(new Date(WEEKvm.Week).getTime() / oneDayTime);
            var now_other = parseInt(new Date().getTime() / oneDayTime);
            return parseInt((old_count + 4) / 7) == parseInt((now_other + 4) / 7);
        }
    }
});