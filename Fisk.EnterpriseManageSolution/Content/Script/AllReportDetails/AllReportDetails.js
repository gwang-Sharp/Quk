
var vm = new Vue({
    el: "#allreport",
    data: {
        Week: new Date(),
        Person: "",
        allProjectDetails: {},
        SearchBar: {
            Labledisplay: 'searchshow',
            adisplay: 'searchhide'
        }
    },
    mounted: function () {
        var v = $("#DataStore").val();
        var details = JSON.parse(v);
        this.Person = sessionStorage.getItem("name");
        if (!details.success) {
            setTimeout(function () {
                $.toast(details.msg, "cancel");
            }, 1200);
        }
        else {
            this.allProjectDetails = details;
            //console.log(this.allProjectDetails);
        }
    },
    methods: {
        TimestampToDate: function (obj) {
            var date = '';
            if (obj.toString().indexOf("/Date(") > -1) {
                var TimestampMinseconds = obj.toString().replace("/Date(", "").replace(")/", "");//获取时间戳毫秒数
                var length = TimestampMinseconds.length;
                date = new Date(parseInt(TimestampMinseconds));
                //判断时间戳是否不足13位，不足时低位补0，即乘以10的所差位数次方
                if (length < 13) {
                    var sub = 13 - length;
                    sub = Math.pow(10, sub);//计算10的n次方
                    date = new Date(TimestampMinseconds * sub);
                }
            }
            else {
                date = new Date(obj);
            }
            var datetime = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
            return datetime;
        },
        onSubmit: function () {
            var time = vm.DateUpdate();
            $.ajax({
                url: "/Main/GetDetail",
                type: "post",
                data: {
                    week: time,
                    name: vm.Person
                },
                dateType: "json",
                beforeSend: function () {
                    loading = vm.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    if (data.success) {
                        vm.allProjectDetails = data;
                    }
                    else {
                        $.toast.prototype.defaults.duration = 600;
                        $.toast(data.msg, "cancel");
                    }
                }
            });
        },
        DateUpdate: function () {
            var date = new Date(vm.Week);
            var dayTime = date.getDate();
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
            }
            var month = date.getMonth() + 1;
            var datetime = date.getFullYear() + '-' + (month < 10 ? "0" + month : month) + '-' + (dayTime < 10 ? "0" + dayTime : dayTime);
            return datetime;
        },
        Search: function () {
            this.SearchBar.Labledisplay = 'searchhide';
            this.SearchBar.adisplay = 'searchshow';
        },
        SearchReset: function () {
            this.SearchBar.Labledisplay = 'searchshow';
            this.SearchBar.adisplay = 'searchhide';
            this.Person = '';
        }
    }
})