var pro = new Vue({
    el: "#ProManage",
    data: {
        dialogVisible: false,
        pagination: {
            currentpage: 1,
            pagesizes: [10, 20, 30, 40],
            pagesize: 10,
            allNum: 0
        },
        ProList: [],
        ProStatus: [],
        ProTypes: [],
        CustomersList: [],
        ProPmList: [],
        CompanyMemberlist: [],
        ProDepartList: [],
        ProChildDepartList: [],
        FormModel: {
            Id: '',
            CustomerId: '',
            CustomerName: '',
            ProjectId: '',
            ProjectName: '',
            ProjectTypeId: '',
            ContractNumber: '',
            ContractDays: '',
            SigningTime: new Date(),
            PMid: '',
            PMname: '',
            PMdepartmentID: '',
            StartTime: new Date(),
            EstimateEndTime: new Date(),
            ActualEndTime: '',
            ProjectStatusId: '',
            ActualManDay: '',
            ProjectType: '',
            ProjectStatus: ''
        },
        PMmodel: {
            DepartName: '技术部',
            ChildrenDepartName: ''
        },
        SearchContent: {
            ProName: '',
            ContractNum: '',
            name: '',
            startTime: '',
            endTime: '',
            PM: ''
        },
        urlChange: false,
        dialogVisible: false,
        Title: ''
    },
    mounted: function () {
        var v = JSON.parse($("#DataStore").val());
        this.ProList = v.list;
        //console.log(this.ProList);
        this.pagination.allNum = v.Count;
    },
    methods: {
        TableReload: function () {
            $.ajax({
                url: "/Admin/PageNation",
                type: "Post",
                dataType: "json",
                data: {
                    page: 1,
                    size: pro.pagination.pagesize
                },
                beforeSend: function () {
                    loading = pro.$loading({
                        text: "正在拼命加载....."
                    });
                },
                success: function (data) {
                    loading.close();
                    pro.ProList = data.list;
                    pro.pagination.allNum = data.Count;
                    pro.pagination.currentpage = 1;
                }
            });
        },
        resetForm: function () {
            //pro.$refs.FormModel.resetFields();
            pro.FormModel.Id = '';
            pro.FormModel.CustomerId = '';
            pro.FormModel.CustomerName = '';
            pro.FormModel.ProjectId = '';
            pro.FormModel.ProjectName = '';
            pro.FormModel.ProjectTypeId = '';
            pro.FormModel.ContractNumber = '';
            pro.FormModel.ContractDays = 0;
            pro.FormModel.SigningTime = new Date();
            pro.FormModel.PMid = '';
            pro.FormModel.PMname = '';
            pro.FormModel.PMdepartmentID = '';
            pro.FormModel.StartTime = new Date();
            pro.FormModel.EstimateEndTime = new Date();
            pro.FormModel.ActualEndTime = '';
            pro.FormModel.ProjectStatusId = '';
            pro.FormModel.EstimateManDay = 0;
            pro.FormModel.ActualManDay = 0;
            pro.FormModel.ProjectType = '';
            pro.FormModel.ProjectStatus = '';
        },
        handleEdit: function (index, row) {
            //console.log(row);
            EditOrAddLoading = pro.$loading({
                text: "请稍等....."
            });
            //console.log(row);
            pro.Title = '编辑项目';
            //pro.FormModel = row;
            pro.FormModel.Id = row.Id;
            pro.FormModel.CustomerId = row.CustomerId;
            pro.FormModel.CustomerName = row.CustomerName;
            pro.FormModel.ProjectId = row.ProjectId;
            pro.FormModel.ProjectName = row.ProjectName;
            pro.FormModel.ProjectTypeId = row.ProjectTypeId;
            pro.FormModel.ContractNumber = row.ContractNumber;
            pro.FormModel.ContractDays = row.ContractDays;
            pro.FormModel.SigningTime = row.SigningTime;
            pro.FormModel.PMid = row.PMid;
            pro.FormModel.PMname = row.PMname;
            pro.FormModel.PMdepartmentID = row.PMdepartmentID;
            pro.FormModel.StartTime = row.StartTime;
            pro.FormModel.EstimateEndTime = row.EstimateEndTime;
            pro.FormModel.ActualEndTime = row.ActualEndTime;
            pro.FormModel.ProjectStatusId = row.ProjectStatusId;
            pro.FormModel.ProjectType = row.ProjectType;
            pro.FormModel.ProjectStatus = row.ProjectStatus;
            pro.FormModel.ActualManDay = row.ActualManDay;
            pro.GetDictionary();
            pro.getPmList();
        },
        handleDelete: function (index, row) {
            //console.log(index, row);
            pro.$confirm('是否删除该项目', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                $.ajax({
                    url: "/Admin/ProDelete",
                    type: "post",
                    data: {
                        proID: row.Id
                    },
                    dataType: "json",
                    success: function (data) {
                        pro.TableReload();
                        pro.$message({
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
        handleInfo: function (index, row) {
            //console.log(row);
            var iframe = top.$("#iframe");
            $(iframe).attr("src", "/admin/ProjectDetail/" + row.ProjectId);
        },
        handleSizeChange(val) {
            pro.pagination.pagesize = val;
            var url = "";
            var postData = {};
            if (pro.urlChange) {
                url = "/Admin/Search";
                postData = {
                    CustomerName: pro.SearchContent.name,
                    ContractNumber: pro.SearchContent.ContractNum,
                    ProjectName: pro.SearchContent.ProName,
                    startTime: pro.SearchContent.startTime,
                    endTime: pro.SearchContent.endTime,
                    PM: pro.SearchContent.PM,
                    page: pro.pagination.currentpage,
                    size: pro.pagination.pagesize
                };
            }
            else {
                url = "/Admin/PageNation";
                postData = {
                    page: pro.pagination.currentpage,
                    size: pro.pagination.pagesize
                };
            }
            $.ajax({
                url: url,
                type: "Post",
                dataType: "json",
                data: postData,
                success: function (data) {
                    pro.ProList = data.list;
                    pro.pagination.allNum = data.Count;
                }
            });
        },
        handleCurrentChange(val) {
            pro.pagination.currentpage = val;
            var url = "";
            var postData = {};
            if (pro.urlChange) {
                url = "/Admin/Search";
                postData = {
                    CustomerName: pro.SearchContent.name,
                    ContractNumber: pro.SearchContent.ContractNum,
                    ProjectName: pro.SearchContent.ProName,
                    startTime: pro.SearchContent.startTime,
                    endTime: pro.SearchContent.endTime,
                    PM: pro.SearchContent.PM,
                    page: pro.pagination.currentpage,
                    size: pro.pagination.pagesize
                };
            }
            else {
                url = "/Admin/PageNation";
                postData = {
                    page: pro.pagination.currentpage,
                    size: pro.pagination.pagesize
                };
            }
            $.ajax({
                url: url,
                type: "Post",
                dataType: "json",
                data: postData,
                success: function (data) {
                    pro.ProList = data.list;
                    pro.pagination.allNum = data.Count;
                }
            });
        },
        CreateProject: function () {
            //pro.resetForm('FormModel')
            EditOrAddLoading = pro.$loading({
                text: "请稍等....."
            });
            pro.Title = '创建项目';
            if (!!pro.FormModel.CustomerName) {
                //pro.$refs.FormModel.resetFields();
                pro.FormModel.Id = '';
                pro.FormModel.CustomerId = '';
                pro.FormModel.CustomerName = '';
                pro.FormModel.ProjectId = '';
                pro.FormModel.ProjectName = '';
                pro.FormModel.ProjectTypeId = '';
                pro.FormModel.ContractNumber = '';
                pro.FormModel.ContractDays = '';
                pro.FormModel.SigningTime = '';
                pro.FormModel.PMid = '';
                pro.FormModel.PMname = '';
                pro.FormModel.PMdepartmentID = '';
                pro.FormModel.StartTime = '';
                pro.FormModel.EstimateEndTime = '';
                pro.FormModel.ActualEndTime = '';
                pro.FormModel.ProjectStatusId = '';
                pro.FormModel.EstimateManDay = '';
                pro.FormModel.ProjectType = '';
                pro.FormModel.ProjectStatus = '';
            }
            pro.FormModel.ProjectStatus = '';
            pro.FormModel.Id = '';
            pro.PMmodel.ChildrenDepartName = '';
            pro.FormModel.PMdepartmentID = '';
            pro.GetDictionary();
            pro.getPmList();
        },
        ExcelGet: function () {
            window.open("/admin/reportData");
        },
        AddOrUpdate: function () {
            //console.log(pro.PMmodel)
            if (!pro.FormModel.CustomerName) {
                pro.$message.error({
                    message: "请选择客户",
                    duration: 1000
                });
                return;
            }
            if (!pro.FormModel.ProjectType) {
                pro.$message.error({
                    message: "请选择项目类型",
                    duration: 1000
                });
                return;
            }
            if (!pro.FormModel.PMname) {
                pro.$message.error({
                    message: "请选择项目经理",
                    duration: 1000
                });
                return;
            }
            if (!pro.FormModel.ProjectStatus) {
                pro.$message.error({
                    message: "请选择项目状态",
                    duration: 1000
                });
                return;
            }
            var url = "";
            if (!pro.FormModel.Id) {
                url = "/Admin/CreateProject";
                delete pro.FormModel.Id;
            }
            else {
                url = "/Admin/UpdateProject";
            }
            pro.FormModel.PMid = pro.ProPmList.filter(p => {
                return p.name == pro.FormModel.PMname;
            })[0].userid;
            pro.FormModel.CustomerId = pro.CustomersList.filter(c => {
                return c.CustomerName = pro.FormModel.CustomerName;
            })[0].CustomerId;
            if (pro.FormModel.ProjectType != "默认") {
                pro.FormModel.ProjectTypeId = pro.ProTypes.filter(t => {
                    return t.Dic_Type == pro.FormModel.ProjectType;
                })[0].Dic_TypeId;
            }
            if (pro.FormModel.ProjectStatus != "默认") {
                pro.FormModel.ProjectStatusId = pro.ProStatus.filter(s => {
                    return s.Dic_Type == pro.FormModel.ProjectStatus;
                })[0].Dic_TypeId;
            }
            //console.log(pro.FormModel);
            $.ajax({
                url: url,
                type: "post",
                data: {
                    project: JSON.stringify(pro.FormModel)
                },
                dataType: "json",
                beforeSend: function () {
                    loading = pro.$loading({
                        text: "正在拼命提交....."
                    });
                },
                success: function (data) {
                    loading.close();
                    pro.TableReload();
                    pro.$message({
                        type: 'success',
                        message: data.msg,
                        duration: 1000
                    });
                    if (data.msg == "创建成功") {
                        pro.SendMessage(pro.FormModel.PMid);
                    }
                    pro.PMmodel.DepartName = '技术部';
                    pro.urlChange = false;
                    pro.dialogClosed();
                }
            });
        },
        DateUpdate: function (row, column, cellValue, index) {
            var date = new Date(cellValue);
            var dayTime = date.getDate();
            var month = date.getMonth() + 1;
            var datetime = date.getFullYear() + '-' + (month < 10 ? "0" + month : month) + '-' + (dayTime < 10 ? "0" + dayTime : dayTime);
            return datetime;
        },
        GetDictionary: function () {
            $.ajax({
                url: "/Admin/GetDicTionary",
                type: "Post",
                dataType: "json",
                success: function (data) {
                    //console.log(data);
                    if (!data.success) {
                        pro.ProStatus = [];
                        pro.ProTypes = [];
                    }
                    pro.ProStatus = data.StatusDics;
                    pro.ProTypes = data.TypeDics;
                    pro.CustomersList = data.CustomersList;
                }
            });
        },
        getPmList: function () {
            $.ajax({
                url: "/admin/getDepart",
                type: "post",
                dataType: "json",
                success: function (res) {
                    var array = JSON.parse(res);
                    pro.CompanyMemberlist = array;
                    pro.ProDepartList = array.filter(d => {
                        return d.parentid == 1;
                    });
                    var depart = pro.CompanyMemberlist.filter(pm => {
                        return pm.id == pro.FormModel.PMdepartmentID;
                    })[0];
                    if (!!depart && depart.name.toString().indexOf("技术") > -1) {
                        pro.ProChildDepartList = pro.CompanyMemberlist.filter(c => {
                            return c.parentid == 3;
                        });
                    }
                    else {
                        if (!!pro.FormModel.PMdepartmentID) {
                            pro.PMmodel.DepartName = pro.CompanyMemberlist.filter(d => {
                                return d.id == pro.FormModel.PMdepartmentID;
                            })[0].name;
                            pro.ProChildDepartList = [];
                        }
                        else {
                            pro.ProChildDepartList = pro.CompanyMemberlist.filter(c => {
                                return c.parentid == 3;
                            });
                        }
                    }
                    if (!!pro.FormModel.PMdepartmentID) {
                        if (pro.FormModel.PMdepartmentID >= 10 && pro.PMmodel.DepartName == "技术部") {
                            pro.PMmodel.ChildrenDepartName = pro.ProChildDepartList.filter(d => {
                                return d.id == pro.FormModel.PMdepartmentID;
                            })[0].name;
                        }
                        else {
                            pro.PMmodel.DepartName = pro.CompanyMemberlist.filter(d => {
                                return d.id == pro.FormModel.PMdepartmentID;
                            })[0].name;
                            pro.ProChildDepartList = [];
                        }
                        pro.MemberGet();
                    }
                    else {
                        EditOrAddLoading.close();
                        pro.dialogVisible = true;
                    }
                }
            });
        },
        DepartChanged: function () {
            var departID = pro.CompanyMemberlist.filter(d => {
                return d.name == pro.PMmodel.DepartName;
            })[0].id;
            pro.ProChildDepartList = pro.CompanyMemberlist.filter(c => {
                return c.parentid == departID;
            });
            if (pro.ProChildDepartList.length == 0) {
                pro.MemberGet();
            }
        },
        MemberGet: function () {
            var childRenDepartid = '';
            if (pro.ProChildDepartList.length > 0) {
                childRenDepartid = pro.ProChildDepartList.filter(D => {
                    return D.name == pro.PMmodel.ChildrenDepartName;
                })[0].id;
            }
            else {
                childRenDepartid = pro.CompanyMemberlist.filter(d => {
                    return d.name == pro.PMmodel.DepartName;
                })[0].id;
            }
            pro.FormModel.PMdepartmentID = childRenDepartid;
            $.ajax({
                url: "/admin/getDepartMember",
                type: "post",
                dataType: "json",
                data: {
                    departID: childRenDepartid
                },
                beforeSend: function () {
                    loading = pro.$loading({
                        text: "正在拼命拉取....."
                    });
                },
                success: function (data) {
                    loading.close();
                    //console.log(data);
                    var allmembers = JSON.parse(data);
                    pro.ProPmList = allmembers;
                    if (!!pro.FormModel.PMdepartmentID) {
                        var one = pro.ProPmList.filter(o => {
                            return o.name == pro.FormModel.PMname;
                        })[0];
                        if (!!one) {
                            pro.FormModel.PMname = pro.FormModel.PMname;
                        }
                        else {
                            pro.FormModel.PMname = '';
                        }
                    }
                    else {
                        pro.FormModel.PMname = '';
                    }
                    EditOrAddLoading.close();
                    pro.dialogVisible = true;
                    //pro.ProPmList = allmembers.filter(m => {
                    //    return m.position == "项目经理";
                    //});
                    //console.log(pro.ProPmList)
                }
            });
        },
        SearchProject: function () {
            if (!!pro.SearchContent.startTime && !pro.SearchContent.endTime) {
                pro.$message.error({
                    message: "请选择结束时间",
                    duration: 1500
                });
                return;
            }
            if (!pro.SearchContent.startTime && !!pro.SearchContent.endTime) {
                pro.$message.error({
                    message: "请选择开始时间",
                    duration: 1500
                });
                return;
            }
            $.ajax({
                url: "/Admin/Search",
                type: "post",
                data: {
                    CustomerName: pro.SearchContent.name,
                    ContractNumber: pro.SearchContent.ContractNum,
                    ProjectName: pro.SearchContent.ProName,
                    startTime: pro.SearchContent.startTime,
                    endTime: pro.SearchContent.endTime,
                    PM: pro.SearchContent.PM
                },
                success: function (data) {
                    pro.urlChange = true;
                    if (data.Count == 0) {
                        pro.$message({
                            type: 'success',
                            message: "无相关数据",
                            duration: 1500
                        });
                        return;
                    }
                    if (!data.success) {
                        pro.$message({
                            type: 'success',
                            message: data.msg,
                            duration: 1500
                        });
                        return;
                    }
                    else {
                        pro.$message({
                            type: 'success',
                            message: data.msg,
                            duration: 1500
                        });
                    }

                    pro.ProList = data.list;
                    pro.pagination.allNum = data.Count;
                }
            });
        },
        SendMessage: function (userid) {
            var messgBody = {
                "touser": userid,
                "msgtype": "text",
                "agentid": 1000040,
                "text": {
                    "content": "你有新的任务了，请及时查收！！！"
                }
            };
            //console.log(messgBody);
            $.ajax({
                url: "/Main/MessageSend",
                type: "POST",
                data: { msg: JSON.stringify(messgBody) },
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    //pro.$message({
                    //    type: 'success',
                    //    message: "已发送提醒",
                    //    duration: 1000
                    //});
                }
            });
        },
        dialogClosed: function () {
            pro.dialogVisible = false;
            pro.resetForm();
        }
    }
});