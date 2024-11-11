using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// 存放商家需要使用到的函数
/// </summary>
public class ShopkeeperFunctions
{
    /// <summary>
    /// 得到所有未完成的订单明细
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>二维数组</returns>
    static public List<List<string>> GetUnfinishedOrderInfo(string _connectionString)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT fname, amount, dno, ordertime FROM unfinishedbill";
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
    /// 得到所有订单信息和订单完成情况
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns></returns>
    static public List<List<string>> GetOrderInfo(string _connectionString)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT o.ono, ordertime, sumprice * discount, finishtime, income, note FROM orders AS o LEFT JOIN historyorder AS ho ON o.ono = ho.ono";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        List<string> _tmp = new List<string>();
                        for (int _i = 0; _i < 6; _i++)
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
    /// 结算订单，商家对订单进行结算，将订单添加至历史订单，如果商家输入了金额，那么返回商家输入的金额；否则从总价格中扣除所有未上的菜品将，返回顾客实际应付价格
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <param name="_note"></param>
    /// <param name="_income"></param>
    /// <returns></returns>
    static public float FinishOrderByShopkeeper(string _connectionString, string _ONO, string _note = "", float _income = 99999999f)
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
                _command.Parameters.AddWithValue("@iscustomerfinish", 0);
                if (_note != "") _command.Parameters.AddWithValue("@note", _note);
                if (_income != 99999999f) _command.Parameters.AddWithValue("@income", _income);
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
    /// 通过ONO得到DNO，用于移除seating表内信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <returns></returns>
    static public string GetDNOByONO(string _connectionString, string _ONO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT dno FROM orders AS o " +
                                $"LEFT JOIN historyorder AS ho ON o.ono = ho.ono " +
                                $"WHERE o.ono = '{_ONO}' AND finishtime IS NULL";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    _reader.Read();
                    return _reader.GetValue(0).ToString();
                }
                else
                {
                    Debug.Log("严重错误：orders表内没有检索到ono对应的dno");
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
    /// 向desk表插入信息
    /// </summary>
    /// <param name="_connectionString"></param>
    static public void InsetIntoDesk(string _connectionString, string _capacity, string _position)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _newDNO = CustomerFunctions.GetNewNO(_connectionString, "d");
                string _query = $"INSERT INTO desk VALUES('{_newDNO}', {_capacity}, '{_position}', 0)";
                SqlCommand _command = new(_query, _connection);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 从desk表内删除信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_capacity"></param>
    /// <param name="_position"></param>
    static public void DeleteFromDesk(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"DELETE FROM desk WHERE dno = '{_DNO}'";
                SqlCommand _command = new(_query, _connection);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 修改desk表内信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    static public void UpdateDesk(string _connectionString, string _DNO, string _capacity, string _position)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "UPDATE desk SET";
                if (_capacity != "") _query += $" capacity = {_capacity},";
                if (_position != "") _query += $" position = '{_position}',";
                _query = _query.Remove(_query.LastIndexOf(','));
                _query += $" WHERE dno = '{_DNO}'";
                SqlCommand _command = new(_query, _connection);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 向food表插入信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_capacity"></param>
    /// <param name="_position"></param>
    static public void InsetIntoFood(string _connectionString, string _FName, string _kind, string _price, string _img)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _newFNO = CustomerFunctions.GetNewNO(_connectionString, "f");
                string _query = $"INSERT INTO food VALUES('{_newFNO}', '{_FName}', '{_kind}', {_price}, '{_img}')";
                SqlCommand _command = new(_query, _connection);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 向food表修改信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <param name="_capacity"></param>
    /// <param name="_position"></param>
    static public void UpdateFood(string _connectionString, string _FNO, string _FName, string _kind, string _price, string _img)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "UPDATE food SET";
                if (_FName != "") _query += $" fname = '{_FName}',";
                if (_kind != "") _query += $" kind = '{_kind}',";
                if (_price != "") _query += $" price = {_price},";
                if (_img != "") _query += $" img = '{_img}',";
                _query = _query.Remove(_query.LastIndexOf(','));
                _query += $" WHERE fno = '{_FNO}'";
                SqlCommand _command = new(_query, _connection);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 从food表删除信息
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_FNO"></param>
    static public void DeleteFromFood(string _connectionString, string _FNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"DELETE FROM food WHERE fno = '{_FNO}'";
                SqlCommand _command = new(_query, _connection);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 调用存储重新计算热门菜品排名
    /// </summary>
    /// <param name="_connectionString"></param>
    static public void RecalculateHotRank(string _connectionString)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd8_calculatehotrank";
                SqlCommand _command = new(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("捕获错误：" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// 得到热门菜品及其排名
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns></returns>
    static public List<List<string>> GetHotFoodAndRank(string _connectionString)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT newtt.fno, fname, no FROM (SELECT fno, ROW_NUMBER() OVER (ORDER BY salesvolume DESC) AS no " +
                                "FROM (SELECT fno, SUM(amount) AS salesvolume FROM orderdetail GROUP BY fno) AS newt) AS newtt " +
                                "LEFT JOIN food ON food.fno = newtt.fno " +
                                "WHERE no <= 5";
                                //"SELECT fno， no " +
                                //"FROM (" +
                                //        "SELECT fno, ROW_NUMBER() OVER(ORDER BY salesvolume DESC) AS no " +
                                //        "FROM (" +
                                //                "SELECT fno, SUM(amount) AS salesvolume " +
                                //                "FROM orderdetail GROUP BY fno" +
                                //              ") AS newt" +
                                //       ") AS newtt " +
                                //"WHERE no <= 5";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        List<string> _tmp = new List<string>();
                        for (int _i = 0; _i < 3; _i++)
                        {
                            _tmp.Add(_reader.GetValue(_i).ToString());
                        }
                        _returnList.Add(_tmp);
                    }
                    return _returnList;
                }
                else
                {
                    Debug.Log("严重错误：统计热门排名无结果");
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
    /// 得到日营收、月营收或年营收
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_mode"></param>
    /// <returns></returns>
    static public List<List<string>> GetRevenueInfo(string _connectionString, int _mode = 0)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                if (_mode == 0 || _mode == 1 || _mode == 2)
                {
                    string _query;
                    if (_mode == 0) _query = "SELECT ndate, sumincome, sumamount FROM revenue";
                    else if (_mode == 1) _query = "SELECT CONVERT(varchar(7), ndate, 120), SUM(sumincome), SUM(sumamount) FROM revenue GROUP BY CONVERT(varchar(7), ndate, 120)";
                    else  _query = "SELECT CONVERT(varchar(4), ndate, 120), SUM(sumincome), SUM(sumamount) FROM revenue GROUP BY CONVERT(varchar(4), ndate, 120)";
                    SqlCommand _command = new(_query, _connection);
                    SqlDataReader _reader = _command.ExecuteReader();
                    
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {
                            List<string> _tmp = new List<string>();
                            for (int _i = 0; _i < 3; _i++)
                            {
                                _tmp.Add(_reader.GetValue(_i).ToString());
                            }
                            _returnList.Add(_tmp);
                        }
                        return _returnList;
                    }
                    else
                    {
                        Debug.Log("严重错误：在revenue视图中未检索到任何信息");
                        return null;
                    }
                }
                else
                {
                    Debug.Log("参数输入不合法");
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
    /// 得到时间段内的日营收
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_mode"></param>
    /// <returns></returns>
    static public List<List<string>> GetRevenueInfo(string _connectionString, string _beginDate, string _endDate)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT ndate, sumincome, sumamount FROM revenue WHERE ndate BETWEEN '{_beginDate}' AND '{_endDate}'";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        List<string> _tmp = new List<string>();
                        for (int _i = 0; _i < 3; _i++)
                        {
                            _tmp.Add(_reader.GetValue(_i).ToString());
                        }
                        _returnList.Add(_tmp);
                    }
                    return _returnList;
                }
                else
                {
                    Debug.Log("在revenue视图中未检索到任何信息");
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
