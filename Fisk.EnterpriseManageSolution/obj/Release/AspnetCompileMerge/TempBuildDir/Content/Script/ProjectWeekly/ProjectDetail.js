
var ProjectDetail = new Vue({
    el: '#ProjectDetail',
    data() {
        return {
            Status: '',
            FormData: {
                NaturalWeek: '',
                ProjectWeek: '',
                ProjectName: '',
                TimeProgressCss: 0,
                TimeProgress: 0,
                ProjectProgressCss: 0,
                ProjectProgress: 0,
                ImplementationProgress: 0,
                ContractDays: 0,
                ActualDays: 0,
                WeeklyContent: '',
                Differences: '',
                NextWeekPlans: '',
                ProjectStatus: '请选择状态',
                PersonnelUseCss: 0,
                PersonnelUse: 0,
                ProjectId: ''
            },
            popupVisible: false,
            slots: [{
                values: ["红", "黄", "绿"]
            }],
            memberList: [],
            allCount: 0,
            handler: function (e) { e.preventDefault(); },
            onlyCheck: true,
            IsPrincipal: false,
            DIVdisplay: false
        };
    },
    created: function () {
        this.GetProjectDetail();
    },
    mounted: function () {
        setTimeout(function () {
            Master.TabbarSelected = '';
            Master.BottomMenuDisplay = true;
        }, 0);
        var obj = $('.mint-cell-wrapper')[0];
        var proName = $(obj).children()[1];
        $(proName).css({ "font-weight": "bold", "color": "black" });
        var v = JSON.parse($("#DataStore").val());
        var msg = v.msg;
        if (msg != "查询成功") {
            this.memberList = [];
        }
        else {
            this.memberList = v.list;
            var count = 0;
            $.each(this.memberList, function (index, obj) {
                count += (obj.WorkingTime * 10);
            });
            this.allCount = count / 10;
            sessionStorage.setItem("member", JSON.stringify(this.memberList));
        }
        this.onlyCheck = v.onlyCheck;
        this.IsPrincipal = v.IsPrincipal;
        var u = navigator.userAgent;
        if (v.onlyCheck) {
            if (u.indexOf("Android") > -1 || u.indexOf("Linux") > -1) {
                $('.borderstyle')[1].style.marginBottom = "65px";
            }
            else {
                this.DIVdisplay = true;
            }
            this.IsPrincipal = true;
        }
        if (v.IsPrincipal) {
            this.onlyCheck = false;
            if (u.indexOf("Android") > -1 || u.indexOf("Linux") > -1) {
                $('.borderstyle')[1].style.marginBottom = "65px";
            }
            else {
                this.DIVdisplay = true;
            }
        }
    },
    methods: {
        GetProjectDetail: function () {
            //获取数据
            let that = this;
            $.ajax({
                url: "/Main/GetProjectDetail",
                type: "POST",
                dataType: "JSON",
                success: function (res) {
                    if (res.success) {
                        if (res.data != '') {
                            that.FormData = res.data[0];
                        } else {
                            that.FormData = {};
                        }
                    } else {
                        Message(res.msg, 'error');
                    }
                }
            });
        },
        ShouPup: function () {
            document.getElementsByTagName("body")[0].addEventListener('touchmove',
                this.handler, { passive: false });//阻止默认事件
            this.popupVisible = true;
        },
        onValuesChange: function (picker, values) {
            this.Status = values[0];
        },
        DetailAdd: function () {
            $.toast.prototype.defaults.duration = 500;
            if (this.FormData.ProjectStatus == "请选择状态") {
                $.toast("请选择项目状态", "cancel");
                return;
            }
            $.ajax({
                url: "/main/WeeklyDetailAdd",
                type: "post",
                data: { WeeklyDetail: JSON.stringify(this.FormData) },
                dataType: "json",
                beforeSend: function () {
                    loading = ProjectDetail.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    $.toast(data.msg, function () {
                        window.location.reload(true);
                    });
                }
            });
        },
        makeSure: function () {
            document.getElementsByTagName("body")[0].removeEventListener('touchmove',
                this.handler, { passive: false });//打开默认事件
            this.FormData.ProjectStatus = this.Status;
            this.popupVisible = false;
        },
        cancel: function () {
            document.getElementsByTagName("body")[0].removeEventListener('touchmove',
                this.handler, { passive: false });//打开默认事件
            this.popupVisible = false;
        },
        Onfenpei: function () {
            window.location.href = "/main/GetToken";
        },
        StartGetReport: function () {
            let that = this;
            $.ajax({
                url: "/main/SendEmail",
                type: "POST",
                data: {
                    CustomerName: that.FormData.CustomerName,
                    ProjectName: that.FormData.ProjectName
                },
                beforeSend: function () {
                    loading = ProjectDetail.$loading({
                        text: "正在拼命获取....."
                    });
                },
                dataType: "json",
                success: function (res) {
                    loading.close();
                    if (res.success) {
                        $.toptip(res.msg, 'success');
                        $(".bg-success").css("background-color", "#17af17");
                        setInterval(function () {
                            $(".bg-success").remove();
                        }, 3000);
                        sessionStorage["Start"] = true;
                        var set = setInterval(function () {
                            sessionStorage["Timer"] = parseInt(sessionStorage.getItem("Timer")) - 1;
                            if (parseInt(sessionStorage["Timer"]) == 0) {
                                sessionStorage["Start"] = false;
                                sessionStorage["Timer"] = 30;
                                window.clearInterval(set);
                            }
                        }, 1000);
                    }
                    else {
                        $.toptip(res.msg, 'error');
                        $(".bg-error").css("background-color", "indianred");
                        setInterval(function () {
                            $(".bg-error").remove();
                        }, 3000);
                    }
                }
            });
        },
        MakeReport: function () {
            let that = this;
            if (parseInt(sessionStorage["Timer"]) < 30) {
                $.toast(`请${parseInt(sessionStorage.getItem('Timer'))}秒后再试!`, "cancel");
                return;
            }
            that.StartGetReport();
        },
        SendMessage: function () {
            var touser = "";
            $.toast.prototype.defaults.duration = 800;
            var sendP = this.memberList.filter(m => {
                return m.checked == true || m.checked == "true";
            });
            if (sendP.length == 0) {
                $.toast("请选择人员", "cancel");
                return;
            }
            else if (sendP.length == 1) {
                touser = sendP[0].userid;
            }
            else {
                //console.log(sendP);
                var touserList = new Array();
                $.each(sendP, function (index, obj) {
                    touserList.push(obj.userid);
                });
                touser = touserList.join('|');
            }
            var messgBody = {
                "touser": touser,
                "msgtype": "text",
                "agentid": 1000040,
                "text": {
                    "content": "每日的日报不要忘记了,记得填写每日日报！"
                }
            };
            //console.log(messgBody);
            $.ajax({
                url: "/Main/MessageSend",
                type: "POST",
                data: { msg: JSON.stringify(messgBody) },
                dataType: "json",
                success: function (data) {
                    $.toast("已发送提醒", "success");
                }
            });
        },
        AllChoosed: function (e) {
            var ButtonObj = e.target;
            var text = $(ButtonObj).text();
            if (text == "全选") {
                $(ButtonObj).text("取消");
                $.each(this.memberList, function (index, obj) {
                    obj.checked = true;
                });
            }
            else {
                $(ButtonObj).text("全选");
                $.each(this.memberList, function (index, obj) {
                    obj.checked = false;
                });
            }
            var array = this.memberList;
            this.memberList = [];
            this.memberList = array;
        },
        getInfo: function (e) {
            var name = $(e.target).text();
            if (!name) {
                this.handler(e);
                return;
            }
            if (name.length > 5) {
                var FirstObj = $(e.target).children()[0];
                name = $(FirstObj).text();
            }
            else if (name.toString().indexOf("天") > -1) {
                var nextObj = $(e.target).siblings();
                name = $(nextObj).text();
            }
            var userID = this.memberList.filter(u => {
                return u.name == name;
            })[0].userid;
            this.handler(e);
            sessionStorage.setItem("name", name);
            window.location.href = "/Main/AllReportDetails/" + userID;
        },
        GetChoosed: function (e) {
        },
        DelthisMember: function (UserId) {
            $.post("/main/proTeamDel", { userID: UserId }, res => {
                $.toast.prototype.defaults.duration = 800;
                var v = JSON.parse(res);
                this.memberList = [];
                this.memberList = v.list;
                sessionStorage["member"] = JSON.stringify(this.memberList);
                var count = 0;
                $.each(this.memberList, function (index, obj) {
                    count += (obj.WorkingTime * 10);
                });
                this.allCount = count / 10;
                $.toast("删除成功", "success");
            });
            $('.weui-cell__bd').css("transform", "translate3d(0px, 0px, 0px)");
        },
        CancelthisMember: function (e) {
            sessionStorage["member"] = JSON.stringify(this.memberList);
            $(e.target).parent()[0].previousElementSibling.style.transform = "translate3d(0px, 0px, 0px)";
        },
        CancelPrincipal: function (Isprincipal, userid) {
            var that = this;
            var obj = {};
            obj.userID = userid;
            obj.IsPrincipal = Isprincipal;
            sessionStorage["member"] = JSON.stringify(this.memberList);
            $.post("/main/SetPrincipal", obj, res => {
                if (!res.success) {
                    $.toast(res.msg, "cancel");
                }
                else {
                    var msg = '';
                    if (Isprincipal) {
                        msg = `你被设为${that.FormData.ProjectName}负责人,请完成相关工作！！！`;
                    }
                    else {
                        msg = `你被取消${that.FormData.ProjectName}负责人！！！`;
                    }
                    var messgBody = {
                        "touser": userid,
                        "msgtype": "text",
                        "agentid": 1000040,
                        "text": {
                            "content": msg
                        }
                    };
                    //console.log(messgBody);
                    $.ajax({
                        url: "/Main/MessageSend",
                        type: "POST",
                        data: { msg: JSON.stringify(messgBody) },
                        dataType: "json",
                        success: function (data) {
                            if (Isprincipal) {
                                $.toast("已发送提醒", "success");
                            }
                            else {
                                $.toast("已取消设置", "success");
                            }
                        }
                    });
                }
            });
        }
    }
});