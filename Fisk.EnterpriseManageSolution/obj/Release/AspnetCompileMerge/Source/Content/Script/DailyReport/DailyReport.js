
var DAILYvm = new Vue({
    el: "#DailyReport",
    data: {
        Day: new Date(),
        allProjects: [],
        allWeekDays: 0,
        ProDaIlyLogs: [],
        DisAbled: true,
        ISupdate: false
    },
    mounted: function () {
        setTimeout(function () {
            if (Master.Isprincipal != 'False') {
                Master.TabbarSelected = 'Personal';
                Master.BottomMenuDisplay = true;
                $(".container")[0].style.marginBottom = "70px";
            }
        }, 0);
        var v = JSON.parse($("#DataStore").val());
        this.allProjects = v.Pros;
        this.dailyWork = this.allProjects[0][0].Remark;
        var HasDailyReport = v.Edit;
        if (HasDailyReport) {
            this.ProDaIlyLogs = this.allProjects;
            this.ISupdate = true;
        }
    },
    methods: {
        UpdateDailyLogs: function () {
            $.toast.prototype.defaults.duration = 800;
            if (!DAILYvm.IsSameWeek()) {
                $.toast("只能对本周进行编辑！", "forbidden");
                return;
            }
            if (!DAILYvm.IsTomorrow()) {
                $.toast("不可填写当前日报", "forbidden");
                return false;
            }
            $.each(DAILYvm.ProDaIlyLogs, function (inde, obj) {
                for (var i = 0; i < obj.length; i++) {
                    DAILYvm.allWeekDays += parseFloat(obj[i].WorkingTime);
                }
            });
            if (DAILYvm.allWeekDays == 0) {
                $.toast("请先选择工时", "cancel");
                return;
            }
            if (DAILYvm.allWeekDays > 1) {
                $.toast("人天总和不得大于一天", "cancel");
                DAILYvm.allWeekDays = 0;
                return;
            }
            var d = DAILYvm.DateUpdate();
            $.ajax({
                url: "/Main/UpdateDailyLog",
                type: "post",
                data: {
                    day: d,
                    proDailyLogs: JSON.stringify(DAILYvm.ProDaIlyLogs)
                },
                dateType: "json",
                beforeSend: function () {
                    loading = DAILYvm.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    $.toast(data.msg, 1000);
                    DAILYvm.allWeekDays = 0;
                }
            });
        },
        LoadDailyLogs: function () {
            DAILYvm.allProjects = [];
            var length = DAILYvm.ProDaIlyLogs.length;
            DAILYvm.ProDaIlyLogs.splice(0, length);
            var d = DAILYvm.DateUpdate();
            $.ajax({
                url: "/Main/LoadDailyLog",
                type: "post",
                dataType: "json",
                data: { day: d },
                beforeSend: function () {
                    loading = DAILYvm.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    DAILYvm.ProDaIlyLogs = data;
                    DAILYvm.ISupdate = true;
                    var hid = document.getElementById("daily").style.display;
                    if (hid == "block") {
                        $("#daily").toggle();
                        $("#dailyLog").toggle();
                    }
                }
            });
        },
        DateUpdate: function () {
            var date = new Date(DAILYvm.Day);
            var month = date.getMonth() + 1;
            var today = date.getDate();
            return date.getFullYear() + '-' + (month < 10 ? "0" + month : month) + '-' + (today < 10 ? "0" + today : today);
        },
        BeforeSubmit: function () {
            $.toast.prototype.defaults.duration = 800;
            if (!DAILYvm.IsTomorrow()) {
                $.toast("不可填写当前日报", "forbidden");
                return false;
            }
            $.each(DAILYvm.allProjects, function (index, obj) {
                for (var i = 0; i < obj.length; i++) {
                    DAILYvm.allWeekDays += parseFloat(obj[i].WorkingTime);
                }
            });
            if (DAILYvm.allWeekDays == 0) {
                $.toast("请先选择工时", "cancel");
                return false;
            }
            if (DAILYvm.allWeekDays > 1) {
                $.toast("人天总和不得大于一天", "cancel");
                DAILYvm.allWeekDays = 0;
                return false;
            }

            var Nowday = DAILYvm.DateUpdate();
            $.each(DAILYvm.allProjects, function (index, obj) {
                for (var a = 0; a < obj.length; a++) {
                    obj[a].ThatDay = Nowday;
                }
            });
            return true;
        },
        DailyRecordLogAdd: function () {
            if (DAILYvm.ISupdate) {
                DAILYvm.UpdateDailyLogs();
                return;
            }
            if (DAILYvm.BeforeSubmit()) {
                $.ajax({
                    url: "/Main/DailyRecordLog",
                    type: "post",
                    data: { proDailyLog: JSON.stringify(DAILYvm.allProjects) },
                    dataType: "json",
                    beforeSend: function () {
                        loading = DAILYvm.$loading({
                            text: "正在拼命提交....."
                        });
                    },
                    success: function (data) {
                        DAILYvm.ProDaIlyLogs = DAILYvm.allProjects;
                        loading.close();
                        $.toast(data.msg, 1000);
                        DAILYvm.allWeekDays = 0;
                        DAILYvm.ISupdate = true;
                    }
                });
            }
        },
        DailyRecordLogReset: function () {
            if (!DAILYvm.IsSameWeek()) {
                $.toast("只能对本周进行编辑！", "forbidden");
                return;
            }
            if (DAILYvm.allProjects.length > 0) {
                $.each(DAILYvm.allProjects, function (inde, obj) {
                    for (var i = 0; i < obj.length; i++) {
                        obj[i].WorkingTime = 0;
                        obj[i].Remark = "";
                    }
                });
            }
            if (DAILYvm.ProDaIlyLogs.length > 0) {
                $.each(DAILYvm.ProDaIlyLogs, function (inde, obj) {
                    for (var i = 0; i < obj.length; i++) {
                        obj[i].WorkingTime = 0;
                        obj[i].Remark = "";
                    }
                });
            }
            $(".is-dark").hide();
            var day = DAILYvm.DateUpdate();
            $.ajax({
                url: "/Main/DailyRecordReset",
                type: "POST",
                data: { today: day },
                success: function (res) {
                    if (res.success) {
                        $.toast(res.msg, 1000);
                    }
                    else {
                        $.toast(res.msg, "cancel");
                    }
                }
            });
        },
        beforeDate: function () {
            this.DisAbled = false;
            this.Day = new Date(this.Day.setDate(this.Day.getDate() - 1));
            this.LoadDailyLogs();
        },
        afterDate: function () {
            this.Day = new Date(this.Day.setDate(this.Day.getDate() + 1));
            if (new Date() <= new Date(this.Day)) {
                this.DisAbled = true;
            }
            this.LoadDailyLogs();
        },
        LoadDailyRecord: function () {
            this.DisAbled = false;
            this.LoadDailyLogs();
        },
        IsTomorrow: function () {
            var date = DAILYvm.DateUpdate();
            var year = new Date().getFullYear();
            var month = new Date().getMonth() + 1;
            var day = new Date().getDate();
            var newdate = year + '-' + (month < 10 ? "0" + month : month) + '-' + (day < 10 ? "0" + day : day);
            if (new Date(newdate) < new Date(date)) {
                return false;
            }
            else {
                return true;
            }
        },
        IsSameWeek: function () {
            //不带时区计算
            //getTime()返回毫秒数，不带时区
            var oneDayTime = 1000 * 60 * 60 * 24;//一天的毫秒数
            var chooseTime = DAILYvm.DateUpdate();
            var reg = new RegExp("-", "g");//replace默认只替换一次，这里设置全局替换
            chooseTime = chooseTime.replace(reg, "/");//统一换成不带时区格式
            var old_count = parseInt(new Date(chooseTime).getTime() / oneDayTime);
            var today = new Date();
            var dayOfWeek = today.getDay();
            var todayYear = today.getFullYear();
            var todayMonth = today.getMonth();
            var todayDate = today.getDate();
            today = new Date(todayYear, todayMonth, todayDate);//2020/01/15格式，不带时区
            var now_other = parseInt(today.getTime() / oneDayTime);
            return parseInt((old_count + 4) / 7) == parseInt((now_other + 4) / 7);
        }
    }
});