using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// 存放顾客使用的函数
/// </summary>
 public class CustomerFunctions
{
    private string justForTest = "This is just for test";

    /// <summary>
    /// 根据CNO取得该用户所有信息，并更新相关引用变量
    /// </summary>
    /// <param name="_connectionString">数据库连接字符串</param>
    /// <param name="_CNO">顾客编号</param>
    /// <param name="_name">姓名</param>
    /// <param name="_password">密码</param>
    /// <param name="_sex">性别</param>
    /// <param name="_contactWay">联系方式</param>
    /// <param name="_VIPRank">VIP等级，如果无VIP，则返回空字符串</param>
    static public void UpdatePersonalInfo(string _connectionString, string _CNO, ref string _name, ref string _password, ref string _sex, ref string _contactWay, ref string _VIPRank)
    {
        {
            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    string _query = "SELECT cname, password, sex, contactway, viprank, " +
                                    "CASE WHEN DATEDIFF(DAY, upgradedate, GETDATE()) > duration OR viprank IS NULL THEN 0 ELSE 1 END " +
                                    "FROM customer AS c " +
                                    "LEFT JOIN vipcustomer AS vc ON c.cno = vc.cno " +
                                    $"WHERE c.cno = '{_CNO}'";
                    SqlCommand _command = new SqlCommand(_query, _connection);
                    SqlDataReader _reader = _command.ExecuteReader();

                    if (_reader.HasRows)
                    {
                        _reader.Read();
                        _name = _reader.GetValue(0).ToString();
                        _password = _reader.GetValue(1).ToString();
                        _sex = _reader.GetValue(2).ToString();
                        _contactWay = _reader.GetValue(3).ToString();
                        if (_reader.GetValue(5).ToString() == "1") _VIPRank = _reader.GetValue(4).ToString();
                        else _VIPRank = "无";
                    }
                    else Debug.Log("严重错误：未检索到任何数据");
                }
                catch (System.Exception _ex)
                {
                    Debug.Log("捕获错误：" + _ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// 注册一个顾客账户，即向customer表插入一条新的顾客信息
    /// </summary>
    /// <param name="_connectionString">数据库连接字符串</param>
    /// <param name="_name">姓名</param>
    /// <param name="_password">密码</param>
    /// <param name="_sex">性别</param>
    /// <param name="_contactway">联系方式</param>
    /// <returns>顾客编号，如果出错返回"-1"</returns>
    static public string TryRegisterCustomerAccount(string _connectionString, string _name, string _password, string _sex, string _contactway)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //得到未使用的顾客编号
                string _newCNO = GetNewNO(_connectionString, "c");

                //执行数据插入
                string _query = string.Format("INSERT INTO customer VALUES('{0}', '{1}', '{2}', '{3}', '{4}');", _newCNO, _name, _password, _sex, _contactway);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("账户注册完成，账户ID：" + _newCNO + " 账户拥有者：" + _name);
                return _newCNO;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return "-1";
            }
        }
    }

    /// <summary>
    /// 登录顾客账户，返回布尔值
    /// </summary>
    /// <param name="_connectionString">数据库连接字符串</param>
    /// <param name="_CNO">顾客编号CNO，即Uid</param>
    /// <param name="_password">密码</param>
    /// <returns>登录成功与否</returns>
    static public bool TryLoginCustomerAccount(string _connectionString, string _CNO, string _password)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT cname FROM customer WHERE cno = '{0}' AND password = '{1}'", _CNO, _password);
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows) return true;
                else return false;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// 检查是否可以升级VIP，返回布尔值，如果输出变量持续时间为0，表示捕获错误；如果为-1，表示VIP高级（未到期时）降级为低级，应当提醒用户。
    /// vip升级规则：（1）低级可以升级为高级，持续时间被覆盖（2）同级之间升级，持续时间叠加（3）高级降级为低级，有且仅有vip身份已过期
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO">顾客编号</param>
    /// <param name="_VIPRank">VIP等级</param>
    /// <param name="_duration">VIP的持续时间</param>
    /// <returns>是否可以升级VIP</returns>
    static public bool CanUpgradeVIP(string _connectionString,string _CNO, string _VIPRank, ref int _duration)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //执行有参数和返回值的存储过程
                string _query = "pd4_canupgradevip";
                SqlCommand _command = new SqlCommand(_query, _connection);

                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@cno", _CNO);
                _command.Parameters.AddWithValue("@newviprank", _VIPRank);
                _command.Parameters.AddWithValue("@newduration", _duration);
                SqlParameter _outputDuration = new SqlParameter();
                _outputDuration.SqlDbType = SqlDbType.Int;
                _outputDuration.ParameterName = "@outduration";
                _outputDuration.Direction = ParameterDirection.Output;
                _command.Parameters.Add(_outputDuration);

                _command.ExecuteNonQuery();
                string _tmp = _outputDuration.Value.ToString();
                _duration = int.Parse(_tmp);

                if (_duration == -1) return false;
                else return true;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                _duration = 0;
                return false;
            }
        }
    }

    /// <summary>
    /// 升级VIP用户，即向VIP顾客表插入一条信息，或者修改过期/即将过期的信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO">顾客编号</param>
    /// <param name="_VIPRank">VIP等级</param>
    /// <param name="_duration">VIP持续时间</param>
    static public void UpgradeVIPCustomer(string _connectionString, string _CNO, string _VIPRank, int _duration)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //执行有参数的存储过程
                SqlCommand _command = new SqlCommand("pd1_setvipcustomer", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@cno", _CNO);
                _command.Parameters.AddWithValue("@viprank", _VIPRank);
                _command.Parameters.AddWithValue("@duration", _duration);
                _command.ExecuteNonQuery();

                //Debug.Log("VIP升级完成，当前VIP等级：" + _VIPRank);
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 得到餐桌表信息，以二维字符串列表形式返回，注：餐桌表的isused列不在此二维表中
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>包含餐桌号、容量、位置的二维列表</returns>
    static public List<List<string>> GetDesksInfo(string _connectionString)
    {
        List<List<string>> _infoList = new List<List<string>>();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT dno, capacity, position FROM desk";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("严重错误：GetDesksInfo()中对desk查询返回信息为空");
                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 3; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _infoList.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString()));
                }
                return _infoList;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 刷新得到餐桌表中isused信息，参数需要待刷新的字符串列表
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_deskUsageInfo">布尔列表，表示每张餐桌是否有人使用的</param>
    static public void UpdateDeskUsageInfo(string _connectionString, ref List<bool> _deskUsageInfo)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT isused FROM desk";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("严重错误：GetDesksInfo()中对desk查询返回信息为空");
                _deskUsageInfo.Clear();
                while (_reader.Read())
                {
                    _deskUsageInfo.Add(_reader.GetBoolean(0));
                    //Debug.Log(string.Format(_reader.GetBoolean(0).ToString()));
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 判断是否可以选座
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns></returns>
    static public bool CanChooseDesk(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT isused FROM desk WHERE dno = '{0}'", _DNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows) _reader.Read();
                else
                {
                    Debug.Log("严重错误：CanChooseDesk()中对desk查询返回信息为空");
                    return false;
                }
                if (!_reader.GetBoolean(0)) return true;
                else return false;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// 选座,在seating座位表插入新的信息，同时在desk餐桌表将对应桌子的isused修改为1
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    /// <param name="_DNO"></param>
    static public void ChooseDesk(string _connectionString, string _CNO, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd3_choosedesk";
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@cno", _CNO);
                _command.Parameters.AddWithValue("@dno", _DNO);
                _command.ExecuteNonQuery();

                //Debug.Log("选座完成，顾客" + _CNO + "在" + _DNO + "入座"); 
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 通过餐桌号寻找订单号
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns></returns>
    static public string FindOrder(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT * FROM orders AS o LEFT JOIN historyorder AS ho ON o.ono = ho.ono WHERE dno = '{_DNO}' AND finishtime IS NULL";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    _reader.Read();
                    return _reader.GetValue(0).ToString();
                }
                else return "";
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return "";
            }
        }
    }

    /// <summary>
    /// 判断餐桌是否全部已满
    /// </summary>
    /// <param name="_deskUsageInfo"></param>
    /// <returns></returns>
    static public bool IsDeskFull(List<bool> _deskUsageInfo)
    {
        int _size = _deskUsageInfo.Count;
        for (int _i = 0; _i < _size; _i++)
        {
            if (!_deskUsageInfo[_i]) return false;
        }
        return true;
    }

    /// <summary>
    /// 进入排队，向queuing表插入数据
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    static public void EnterQueue(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("INSERT INTO queuing VALUES('{0}', GETDATE())", _CNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("顾客" + _CNO + "进入排队");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 离开排队，从queuing表删除数据
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    static public void LeaveQueue(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("DELETE FROM queuing WHERE cno = '{0}'", _CNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("顾客" + _CNO + "离开排队");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 叫号，得到空餐桌的编号，然后从queuing检索第一位顾客，判断是否是本机登录用户，如果是，则将在座位表添加信息，并返回真;如果不是，则返回假
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    /// <returns></returns>
    static public bool CallNumber(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT TOP 1 cno FROM queuing ORDER BY queuetime");
                SqlCommand _command1 = new SqlCommand(_query, _connection);
                SqlDataReader _reader1 = _command1.ExecuteReader();

                if (_reader1.HasRows) _reader1.Read();
                else
                {
                    Debug.Log("严重错误：CallNumber()中对queuing查询返回信息为空");
                    return false;
                }
                string _tmpString = _reader1.GetValue(0).ToString();
                _reader1.Close();
                if (_tmpString == _CNO)
                {
                    _query = "SELECT TOP 1 dno FROM desk WHERE isused = 0";
                    SqlCommand _command2 = new SqlCommand(_query, _connection);
                    SqlDataReader _reader2 = _command2.ExecuteReader();

                    if (_reader2.HasRows) _reader2.Read();
                    else
                    {
                        Debug.Log("严重错误：CallNumber()中对desk表查询返回信息为空");
                        return false;
                    }
                    string _DNO = _reader2.GetValue(0).ToString();
                    LeaveQueue(_connectionString, _CNO);
                    ChooseDesk(_connectionString, _CNO, _DNO);
                    return true;

                }
                else return false;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// 得到目标在排队队列的序号
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    /// <returns>排队序号</returns>
    static public string GetQueueSeq(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT no FROM (SELECT cno, ROW_NUMBER() OVER (ORDER BY queuetime) AS no FROM queuing) AS newq WHERE cno = '{_CNO}'";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    _reader.Read();
                    return _reader.GetValue(0).ToString();
                }
                else
                {
                    Debug.Log("严重错误：queuing表内没有检索到对应cno");
                    return null;
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 得到菜单，即从food表中得到所有菜品信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>包含菜品编号、种类、菜品名，价格的二维列表</returns>
    static public List<List<string>> GetFoodInfo(string _connectionString)
    {
        List<List<string>> _foodInfo = new List<List<string>>();
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT fno, kind, fname, price, img FROM food ORDER BY kind, fno";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("严重错误：GetFoodInfo()中对food查询返回信息为空");
                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 5; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _foodInfo.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2} {3}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString(), _reader.GetValue(3).ToString()));
                }
                return _foodInfo;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 得到热门菜品信息，即从bestfood表中得到所有热门菜品信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>包含菜品号、是否热门、热门排名、是否推荐、是否新品的二维列表</returns>
    static public List<List<string>> GetBestFoodInfo(string _connectionString)
    {
        List<List<string>> _bestFoodInfo = new List<List<string>>();
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT fno, ishot, hotranking, isrecommend, isnew FROM bestfood";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("严重错误：GetBestFoodInfo()中对bestfood查询返回信息为空");
                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 5; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _bestFoodInfo.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2} {3} {4}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString(), _reader.GetValue(3).ToString(), _reader.GetValue(4).ToString()));
                }
                return _bestFoodInfo;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 得到菜品子选项，即从availablesuboption表和suboption表中得到该菜品的可选子选项
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_FNO"></param>
    /// <returns>包含子选项编号、类型、名字、价格修正的二维列表</returns>
    static public List<List<string>> GetAvlSuboption(string _connectionString, string _FNO)
    {
        List<List<string>> _avlSuboptionList = new List<List<string>>();
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT asup.sno, kind, sname, priceoffset FROM availablesuboption AS asup " +
                                              "JOIN suboption AS sub ON asup.sno = sub.sno " +
                                              "WHERE fno = '{0}' ORDER BY kind, asup.sno", _FNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                //if (!_reader.HasRows) Debug.Log("严重错误：GetAvailableSuboption()中对availablesuboption和suboption查询返回信息为空");

                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 4; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _avlSuboptionList.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2} {3}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString(), _reader.GetValue(3).ToString()));
                }
                return _avlSuboptionList;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 生成订单，顾客选座成功后自动生成订单
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns>订单号的字符串</returns>
    static public string CreateOrder(string _connectionString, string _DNO)
    {
        string _ONO, _discount;

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //生成订单号
                _ONO = GetNewNO(_connectionString, "o");

                //得到折扣
                _discount = CalculateDiscount(_connectionString, _DNO);
                string _query = string.Format("INSERT INTO orders VALUES('{0}', '{1}', GETDATE(), 0, {2})", _ONO, _DNO, _discount.ToString());
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("订单已生成 订单号:" + _ONO);
                return _ONO;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 计算折扣，从座位表同一座位中取出VIP没有过期且折扣最优惠的作为最佳折扣返回
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns>最佳折扣</returns>
    static public string CalculateDiscount(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //调用有参数有返回值的存储过程
                string _query = "pd2_calculatediscount";
                SqlCommand _command = new SqlCommand(_query, _connection);

                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@dno", _DNO);
                SqlParameter _outputDiscount = new SqlParameter();
                _outputDiscount.ParameterName = "@discount";
                _outputDiscount.SqlDbType = SqlDbType.Float;
                _outputDiscount.Direction = ParameterDirection.Output;
                _command.Parameters.Add(_outputDiscount);

                _command.ExecuteNonQuery();
                string _discount = _outputDiscount.Value.ToString();
                return _discount;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 添加菜品，顾客可以向订单明细中添加菜品
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <param name="_FNO"></param>
    /// <param name="_SNO"></param>
    /// <param name="_amount"></param>
    static public void AddFood(string _connectionString, string _ONO, string _FNO, string _SNO, int _amount)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("INSERT INTO orderdetail VALUES('{0}', '{1}', '{2}', {3}, 1)", _ONO, _FNO, _SNO, _amount);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("菜品已添加至订单");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 得到菜品的信息信息，用于打印
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_FNO"></param>
    /// <param name="_SNO"></param>
    /// <returns></returns>
    static public List<string> GetFoodDetailInfo(string _connectionString, string _FNO, string _SNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query;
                if (_SNO != "s0000")
                {
                    _query = $"SELECT fname, price+ priceoffset, f.kind, img, sname " +
                                $"FROM food AS f " +
                                $"LEFT JOIN availablesuboption AS avls ON avls.fno = f.fno " +
                                $"LEFT JOIN suboption AS s ON s.sno = avls.sno " +
                                $"WHERE f.fno = '{_FNO}' AND s.sno = '{_SNO}'";
                }
                else _query = $"SELECT fname, price, kind, img FROM food WHERE fno = '{_FNO}'";

                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows)
                { 
                    Debug.Log("严重错误：未检索到对应fno和sno的元组");
                    return null;
                }
                else
                {
                    _reader.Read();

                    List<string> _returnList = new();
                    _returnList.Add(_reader.GetValue(0).ToString());
                    _returnList.Add(_reader.GetValue(1).ToString());
                    _returnList.Add(_reader.GetValue(2).ToString());
                    _returnList.Add(_reader.GetValue(3).ToString());
                    if (_SNO == "s0000") _returnList.Add("标准");
                    else _returnList.Add(_reader.GetValue(4).ToString());
                    return _returnList;
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 添加备注，顾客向ordernote添加备注
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <param name="_text"></param>
    static public void AddNote(string _connectionString, string _ONO, string _text)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _NNO = GetNewNO(_connectionString, "n");
                string _query = string.Format("INSERT INTO ordernote VALUES('{0}', '{1}', '{2}')", _NNO, _ONO, _text);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("备注添加完成");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 得到账单信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <returns></returns>
    static public List<List<string>> GetBillInfo(string _connectionString, string _ONO)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT fname, amount, perprice, sumprice FROM bill WHERE ONO = '{_ONO}'";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        List<string> _tmp = new List<string>();
                        for (int _i = 0; _i < 4; _i++)
                        {
                            _tmp.Add(_reader.GetValue(_i).ToString());
                        }
                        _returnList.Add(_tmp);
                    }
                    return _returnList;
                }
                else
                {
                    Debug.Log("严重错误:bill查询的内容为空");
                    return null;
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 结算订单，顾客对订单进行结算，将订单添加至历史订单，同时所有未上的菜品将从总价格中扣除
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <returns>顾客实际应付价格</returns>
    static public float FinishOrderByCustomer(string _connectionString, string _ONO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd5_finishorder";
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@ono", _ONO);
                _command.Parameters.AddWithValue("@iscustomerfinish", 1);
                SqlParameter _outputIncome = new SqlParameter();
                _outputIncome.ParameterName = "@outincome";
                _outputIncome.SqlDbType = SqlDbType.Money;
                _outputIncome.Direction = ParameterDirection.Output;
                _command.Parameters.Add(_outputIncome);
                _command.ExecuteNonQuery();

                float _returnincome = float.Parse(_outputIncome.Value.ToString());
                return _returnincome;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return -1.0f;
            }
        }
    }

    /// <summary>
    /// 离开座位,从seating座位表删除这个座位上所有人信息，同时在desk餐桌表将对应桌子的isused修改为0
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    static public void LeaveDesk(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd7_leavedesk";
                SqlCommand _command = new(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@dno", _DNO);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 测试数据库连接
    /// </summary>
    /// <param name="_connectionString"></param>
    static public void TestDBConnection(string _connectionString)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();
                Debug.Log("数据库连接成功");

                //执行测试查询
                string _query = "SELECT * FROM customer";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                //读取数据
                Show(_reader);
                //while(_reader.Read())
                //{
                //    Debug.Log(_reader.GetValue(0).ToString() + " " + _reader.GetValue(1).ToString());
                //}
            }
            catch (System.Exception _ex)
            {
                Debug.Log("数据库连接失败");
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 从某个表中获得一个未使用的编号并返回序号字符串，第一个编号为x0001或x00000001，最后一个编号为x9999或x999999999
    /// </summary>
    /// <param name="_connectionString">数据库连接字符串</param>
    /// <param name="_tableName">表名</param>
    /// <returns>返回编号字符串，如果出错则返回Wrong字符串</returns>
    static public string GetNewNO(string _connectionString, string _tableName)
    {
        string _query, _firstChar;
        int _size;

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            string _newNO = new string("");
            try
            {
                _connection.Open();

                //判断从哪张表获得编号
                _tableName = _tableName.ToLower();
                if (_tableName == "customer" || _tableName == "c")
                {
                    _query = "SELECT MAX(cno) FROM customer";
                    _size = 8;
                    _firstChar = "c";
                }
                else if (_tableName == "desk" || _tableName == "d")
                {
                    _query = "SELECT MAX(dno) FROM desk";
                    _size = 4;
                    _firstChar = "d";
                }
                else if (_tableName == "order" || _tableName == "o")
                {
                    _query = "SELECT MAX(ono) FROM orders";
                    _size = 8;
                    _firstChar = "o";
                }
                else if (_tableName == "food" || _tableName == "f")
                {
                    _query = "SELECT MAX(fno) FROM food";
                    _size = 4;
                    _firstChar = "f";
                }
                else if (_tableName == "suboption" || _tableName == "s")
                {
                    _query = "SELECT MAX(sno) FROM suboption";
                    _size = 4;
                    _firstChar = "s";
                }
                else if (_tableName == "auditlog" || _tableName == "l")
                {
                    _query = "SELECT MAX(lno) FROM auditlog";
                    _size = 8;
                    _firstChar = "l";
                }
                else if (_tableName == "ordernote" || _tableName == "n")
                {
                    _query = "SELECT MAX(nno) FROM ordernote";
                    _size = 8;
                    _firstChar = "n";
                }
                else return "InputStringIsWrong";

                //执行最大编号查询
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                //读数据
                if (!_reader.HasRows)   //如果为空数据
                {
                    Debug.Log("hereB");
                    _newNO = _firstChar + "1".PadLeft(_size, '0');
                    return _newNO;
                }
                else
                {
                    _reader.Read();
                    if (_reader.GetValue(0).ToString() != "") _newNO = _reader.GetValue(0).ToString();  //如果有有效数据
                    else _newNO = "0000";       //如果不是空数据，但是是空字符串

                    //对编号进行处理，字符串去除标识符转整型，整型+1，整型转字符串加上标识符
                    int _tmpInt = 0;
                    _tmpInt = int.Parse(_newNO.Remove(0, 1));
                    _tmpInt++;
                    if (_size == 8 && _tmpInt > 99999999) return "OutOfIndex";
                    else if (_size == 4 && _tmpInt > 9999) return "OutOfIndex";
                    else
                    {
                        _newNO = _firstChar + _tmpInt.ToString().PadLeft(_size, '0');
                        return _newNO;
                    }
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
                return "Wrong";
            }
        }
    }

    /// <summary>
    /// 展示二维列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    static public void Show<T>(List<List<T>> _list)
    {
        int _rowSize = _list.Count;
        int _columnSize;
        string _str = "";
        for (int _i = 0; _i < _rowSize; _i++)
        {
            _columnSize = _list[_i].Count;
            for (int _j = 0; _j < _columnSize; _j++)
            {
                _str += _list[_i][_j].ToString();
                _str += " ";
            }
            Debug.Log(_str);
            _str = "";
        }
    }

    /// <summary>
    /// 展示reader
    /// </summary>
    /// <param name="_reader"></param>
    static public void Show(SqlDataReader _reader)
    {
        int _columnSize = _reader.FieldCount;
        string str = "";
        while (_reader.Read())
        {
            for (int _i = 0; _i < _columnSize; _i++)
            {
                str += _reader[_i];
                str += " ";
            }
            Debug.Log(str);
            str = "";
        }
    }

    /// <summary>
    /// 标准SQL函数模板
    /// </summary>
    /// <param name="_connectionString"></param>
    static public void StandardSQLFunctionTemplate(string _connectionString)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //Do something here
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }
}
