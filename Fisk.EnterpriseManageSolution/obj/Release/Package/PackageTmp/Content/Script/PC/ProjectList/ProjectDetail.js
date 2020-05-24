


var detail = new Vue({
    el: "#ProjectDetail",
    data: {
        weekdetails: {
            EstimateManDay: 0,
            ActualDays: 0,
            ProjectProgress: 0,
            ProjectHealth: '',
            TimeProgress: 0,
            ImplementationProgress: 0,
            PersonnelUse: 0,
            PersonnelUseData: 0
        },
        weekReport: [],
        proMembers: []
    },
    created: function () {
        this.getAllDetails();
    },
    mounted: function () {

    },
    methods: {
        goBack: function () {
            var iframe = top.$("#iframe");
            $(iframe).attr("src", "/admin/Index");
        },
        getAllDetails: function () {
            let that = this;
            $.ajax({
                url: "/admin/getDetailInfo",
                type: "post",
                dataType: "json",
                success: function (data) {
                    if (!!data.detail) {
                        that.weekdetails = data.detail;
                    }
                    that.weekReport = data.weekReport;
                    that.proMembers = data.proMembers;
                    var progress = $('.el-progress-bar__innerText')[2];
                    $(progress).text(that.weekdetails.PersonnelUse + '%');
                }
            });
        },
        DATEFORMATER: function (row, column, cellValue, index) {
            var date = '';
            var TimestampMinseconds = cellValue.toString().replace("/Date(", "").replace(")/", "");//获取时间戳毫秒数
            var length = TimestampMinseconds.length;
            date = new Date(parseInt(TimestampMinseconds));
            //判断时间戳是否不足13位，不足时低位补0，即乘以10的所差位数次方
            if (length < 13) {
                var sub = 13 - length;
                sub = Math.pow(10, sub);//计算10的n次方
                date = new Date(TimestampMinseconds * sub);
            }
            var datetime = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
            return datetime;
        }
    }
})