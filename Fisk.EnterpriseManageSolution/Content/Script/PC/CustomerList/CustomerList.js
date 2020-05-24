var C = new Vue({
    el: "#CustomerList",
    data: {
        CustomerList: [],
        pagination: {
            currentpage: 1,
            pagesizes: [10, 20, 30, 40],
            pagesize: 10,
            allNum: 0
        },
        SearchContent: {
            name: '',
            TelPhone: ''
        },
        FormModel: {
            Id: '',
            CustomerName: '',
            CustomerAbbreviation: '',
            TaxNumber: '',
            CompanyAddress: '',
            Telephone: '',
            Contacts: '',
            ContactTitle: '',
            ContactPhone: ''
        },
        urlChange: false
    },
    mounted: function () {
        var v = JSON.parse($("#DataStore").val());
        this.CustomerList = v.list;
        //console.log(this.CustomerList);
        this.pagination.allNum = v.Count;
    },
    methods: {
        TableReload: function () {
            $.ajax({
                url: "/Admin/CPageNation",
                type: "Post",
                dataType: "json",
                data: {
                    page: 1,
                    size: C.pagination.pagesize
                },
                beforeSend: function () {
                    loading = C.$loading({
                        text: "正在拼命加载....."
                    });
                },
                success: function (data) {
                    loading.close();
                    C.CustomerList = data.list;
                    C.pagination.allNum = data.Count;
                    C.pagination.currentpage = 1;
                }
            });
        },
        resetForm: function (formName) {
            C.$refs[formName].resetFields();
        },
        AddOrUpdate: function () {
            var url = "";
            if (!C.FormModel.Id) {
                url = "/Admin/CreateCustomer";
                delete C.FormModel.Id;
            }
            else {
                url = "/Admin/UpdateCustomer";
            }
            $.ajax({
                url: url,
                type: "post",
                data: {
                    Customer: JSON.stringify(C.FormModel)
                },
                dataType: "json",
                beforeSend: function () {
                    loading = C.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    layer.closeAll();
                    C.TableReload();
                    C.$message({
                        type: 'success',
                        message: data.msg,
                        duration: 1000
                    });
                    C.resetForm('FormModel');
                }
            });
        },
        SearchCustomer: function () {
            if (!C.SearchCustomer.name && !C.SearchCustomer.TelPhone) {
                C.$message.error({
                    message: "请输入检索内容",
                    duration: 1500
                });
                return;
            }
            $.ajax({
                url: "/Admin/SearchCustomer",
                type: "post",
                data: {
                    CustomerName: C.SearchContent.name,
                    Telephone: C.SearchContent.TelPhone
                },
                success: function (data) {
                    C.urlChange = true;
                    if (data.Count == 0) {
                        C.$message({
                            type: 'success',
                            message: "无相关数据",
                            duration: 1500
                        });
                        return;
                    }
                    if (!data.success) {
                        C.$message({
                            type: 'success',
                            message: data.msg,
                            duration: 1500
                        });
                        return;
                    }
                    C.CustomerList = data.list;
                    C.pagination.allNum = data.Count;
                }
            });
        },
        CreateCustomer: function () {
            this.FormModel.Id = '';
            layui.use('layer', function () {
                var layer = layui.layer;
                layer.open({
                    type: 1,
                    content: $("#form"),
                    area: ["650px", "650px"],
                    title: "创建客户",
                    zIndex: 1999,
                    cancel: function () {
                        C.resetForm('FormModel');
                    }
                });
            });
        },
        handleEdit: function (index, row) {
            this.FormModel = row;
            //console.log(row);
            layui.use('layer', function () {
                var layer = layui.layer;
                layer.open({
                    type: 1,
                    content: $("#form"),
                    area: ["650px", "650px"],
                    title: "客户详情",
                    zIndex: 2000
                });
            });
        },
        handleDelete: function (index, row) {
            //console.log(index, row);
            C.$confirm('是否删除该项目', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                $.ajax({
                    url: "/Admin/CustomerDelete",
                    type: "post",
                    data: {
                        proID: row.Id
                    },
                    dataType: "json",
                    success: function (data) {
                        C.TableReload();
                        C.$message({
                            type: 'success',
                            message: data.msg,
                            duration: 800
                        });
                    }
                });
            }).catch(() => {
                return;
            });
        },
        handleSizeChange(val) {
            C.pagination.pagesize = val;
            var url = "";
            var postData = {};
            if (C.urlChange) {
                url = "/Admin/SearchCustomer";
                postData = {
                    CustomerName: C.SearchContent.name,
                    Telephone: C.SearchContent.TelPhone,
                    page: C.pagination.currentpage,
                    size: C.pagination.pagesize
                };
            }
            else {
                url = "/Admin/CPageNation";
                postData = {
                    page: C.pagination.currentpage,
                    size: C.pagination.pagesize
                };
            }
            $.ajax({
                url: url,
                type: "Post",
                dataType: "json",
                data: postData,
                success: function (data) {
                    C.CustomerList = data.list;
                    C.pagination.allNum = data.Count;
                }
            });
        },
        handleCurrentChange(val) {
            C.pagination.currentpage = val;
            var url = "";
            var postData = {};
            if (C.urlChange) {
                url = "/Admin/SearchCustomer";
                postData = {
                    CustomerName: C.SearchContent.name,
                    Telephone: C.SearchContent.TelPhone,
                    page: C.pagination.currentpage,
                    size: C.pagination.pagesize
                };
            }
            else {
                url = "/Admin/CPageNation";
                postData = {
                    page: C.pagination.currentpage,
                    size: C.pagination.pagesize
                };
            }
            $.ajax({
                url: url,
                type: "Post",
                dataType: "json",
                data: postData,
                success: function (data) {
                    C.CustomerList = data.list;
                    C.pagination.allNum = data.Count;
                }
            });
        }
    }
});