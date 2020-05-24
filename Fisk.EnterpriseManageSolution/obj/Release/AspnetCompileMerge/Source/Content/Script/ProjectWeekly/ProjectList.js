var ProjectList = new Vue({
    el: '#ProjectList',
    computed: {
        FormObj() {
            return {
                page: this.Pagination.page,
                limit: this.Pagination.limit,
                field: this.Pagination.field,
                order: this.Pagination.order,
                SearchContent: this.form.SearchContent,
                UserId: ''
            };
        },
        noMore() {
            return this.ProjectData.length == this.Pagination.count;
        }
    },
    data() {
        return {
            form: {
                SearchContent: ''
            },
            Pagination: {
                page: 1,
                limit: 10,
                pageSizes: [10, 20, 30, 40, 50],
                count: 1,
                field: 'CreateTime',
                order: 'desc'
            },
            ProjectData: [],
            SearchBar: {
                Labledisplay: 'searchshow',
                adisplay: 'searchhide'
            },
            projectID: '',
            loading: true,
            WaitIing: false
        };
    },
    created: function () {
        this.GetProjectData();
    },
    mounted: function () {
        setTimeout(function () {
            Master.TabbarSelected = 'List';
            Master.BottomMenuDisplay = true;
        }, 0);

        window.addEventListener('scroll', this.handleScroll, true);
    },
    methods: {
        DataRowDetail: function (data) {
            this.projectID = data.ProjectId;
            window.location.href = "/Main/ProjectDetail/" + this.projectID;
            return false;
        },
        GetProjectData: function () {
            //获取数据
            let that = this;
            $.post('/Main/GetProjectList', this.FormObj, res => {
                $.toast.prototype.defaults.duration = 500;
                if (res.success) {
                    if (res.data != '') {
                        that.ProjectData = res.data;
                    } else {
                        $.toast(res.message, "cancel");
                    }
                    that.Pagination.count = res.count;
                    if (that.ProjectData.length == that.Pagination.count) {
                        that.loading = false;
                    }
                    return false;
                } else {
                    $.toast(res.message, "cancel");
                    return false;
                }
            });
        },
        onSubmit: function () {
            //筛选
            this.Pagination.page = 1;
            this.GetProjectData();
        },
        Search: function () {
            this.SearchBar.Labledisplay = 'searchhide';
            this.SearchBar.adisplay = 'searchshow';
            $("#searchInput").focus();
        },
        SearchReset: function () {
            this.SearchBar.Labledisplay = 'searchshow';
            this.SearchBar.adisplay = 'searchhide';
            this.form.SearchContent = '';
            this.onSubmit();
        },
        SearchStart: function () {
            this.onSubmit();
        },
        SearchOnBlur: function () {
            this.SearchBar.Labledisplay = 'searchshow';
            this.SearchBar.adisplay = 'searchhide';
            this.form.SearchContent = '';
        },
        searchTo: function (event) {
            if (event.keyCode == 13) {
                this.SearchStart();
            }
        },
        load: function () {
            let that = this;
            that.WaitIing = true;
            that.Pagination.page += 1;
            $.post('/Main/GetProjectList', that.FormObj, res => {
                that.WaitIing = false;
                that.Pagination.count = res.count;
                that.ProjectData = that.ProjectData.concat(res.data);
                if (that.ProjectData.length == that.Pagination.count) {
                    that.loading = false;
                }
            });
        },
        handleScroll: function () {
            //变量scrollTop是滚动条滚动时，距离顶部的距离
            var scrollTop = document.documentElement.scrollTop;
            //变量windowHeight是可视区的高度
            var windowHeight = document.documentElement.clientHeight;
            //变量scrollHeight是滚动条的总高度
            var scrollHeight = document.documentElement.scrollHeight;
            //滚动条到底部的条件
            if (scrollTop + windowHeight >= scrollHeight) {

                if (this.ProjectData.length == this.Pagination.count)
                    return;
                else if (this.WaitIing) {
                    return;
                }
                else {
                    this.load();
                }
            }
        }
    }
});
