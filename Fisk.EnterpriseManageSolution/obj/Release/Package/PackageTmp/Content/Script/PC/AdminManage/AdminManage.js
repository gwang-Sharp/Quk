
var vm = new Vue({
    el: "#app",
    data: {
        NavMenus: [],
        FirstNavMenus: []
    },
    created: function () {
        this.GetNavMenus();
    },
    mounted: function () {
        if (!!this.NavMenus) {
            setTimeout(function () {
                $("#iframe").attr("src", "/admin/index");
            }, 1500);
        }
    },
    methods: {
        GetNavMenus: function () {
            let that = this;
            $.post("/admin/GetNavMenus", function (res) {
                that.NavMenus = res.nav;
                //console.log(res);
            }, "json");
        },
        iframeChangeUrl: function (item) {
            //console.log(item);
            $("#iframe").attr("src", item.MenuUrl);
        },
        changeFrameHeight: function () {
            var ifm = document.getElementById("iframe");
            $(ifm).height(top.$(ifm).parent().height() + 39);
        }
    }
});