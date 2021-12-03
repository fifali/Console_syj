using System.Runtime.Serialization;
using System.Collections.Generic;
namespace ConsoleHydee
{
    [DataContract]
    #region B2B请求消息明细
    public class ProductList
    {
        [DataMember(Order = 0)]
        public string orderItemId { get; set; }//订单编号细单id

        [DataMember(Order = 1)]
        public string productCode { get; set; }//货品id

        [DataMember(Order = 2)]
        public string amount { get; set; }//数量

        [DataMember(Order = 3)]
        public string price { get; set; }//单价

        [DataMember(Order = 4)]
        public string money { get; set; }//金额

        [DataMember(Order = 5)]
        public string batchNumber { get; set; }//批号
    }
    #endregion
    #region 航天金税请求消息
    public class HeadList
    {
        [DataMember(Order = 0)]
        public string xsddm { get; set; }//单据号
        [DataMember(Order = 1)]
        public string fph { get; set; }//发票号
        [DataMember(Order = 2)]
        public string fpdm { get; set; }//发票代码
        [DataMember(Order = 3)]
        public string fplx { get; set; }//发票类型
        [DataMember(Order = 4)]
        public string kprq { get; set; }//开票日期
        [DataMember(Order = 5)]
        public List<DetailList> detail { get; set; }//单据明细ID列表
    }
    public class DetailList
    {
        [DataMember(Order = 0)]
        public string cpdm { get; set; }//订单编号细单id
        [DataMember(Order = 1)]
        public string cpmc { get; set; }//产品名称
        [DataMember(Order = 2)]
        public string cpxh { get; set; }//产品型号
        [DataMember(Order = 3)]
        public string cpdw { get; set; }//产品单位
        [DataMember(Order = 4)]
        public string cpsl { get; set; }//产品数量
        [DataMember(Order = 5)]
        public string bhsdj { get; set; }//不含税单价
        [DataMember(Order = 6)]
        public string hsdj { get; set; }//含税单价
        [DataMember(Order = 7)]
        public string bhsje { get; set; }//不含税金额
        [DataMember(Order = 8)]
        public string hsje { get; set; }//含税金额
        [DataMember(Order = 9)]
        public string se { get; set; }//税额
        [DataMember(Order = 10)]
        public string sl { get; set; }//税率
        [DataMember(Order = 11)]
        public string zdyzd1 { get; set; }//预留字段1
        [DataMember(Order = 12)]
        public string zdyzd2 { get; set; }//预留字段2
    }
    #endregion
    #region 零售请求消息
    public class SALEHeadList
    {
        [DataMember(Order = 0)]
        public string compid { get; set; }//公司编码
        [DataMember(Order = 1)]
        public string busno { get; set; }//门店编码
        [DataMember(Order = 2)]
        public string srcbillno { get; set; }//单据号
        [DataMember(Order = 3)]
        public string salers { get; set; }//收银员工号，没有可为空
        [DataMember(Order = 4)]
        public string netprice { get; set; }//单据总价
        [DataMember(Order = 5)]
        public string accdate { get; set; }//记账日期
        [DataMember(Order = 6)]
        public string paytype { get; set; }//收费类别，多个用分号隔开
        [DataMember(Order = 7)]
        public string payamt { get; set; }//各收费类别对应收费金额，多个用分号隔开，需要和收费类别顺序对应
        [DataMember(Order = 8)]
        public string memcard { get; set; }//会员卡号
        [DataMember(Order = 9)]
        public List<SALEDetailList> detail { get; set; }//单据明细ID列表
    }
    public class SALEDetailList
    {
        [DataMember(Order = 0)]
        public string rowno { get; set; }//行号
        [DataMember(Order = 1)]
        public string wareid { get; set; }//商品编码
        [DataMember(Order = 2)]
        public string wareqty { get; set; }//数量
        [DataMember(Order = 3)]
        public string wareprice { get; set; }//单价
        [DataMember(Order = 4)]
        public string wareamt { get; set; }//金额
        [DataMember(Order = 5)]
        public string batid { get; set; }//批次号，可为空
        [DataMember(Order = 6)]
        public string makeno { get; set; }//生产批号
        [DataMember(Order = 7)]
        public string stdprice { get; set; }//标价
        [DataMember(Order = 8)]
        public string netpriced { get; set; }//实价
        [DataMember(Order = 9)]
        public string purprice { get; set; }//进价
        [DataMember(Order = 10)]
        public string salersd { get; set; }//营业员
    }
    #endregion
    #region 售药机补退货单据请求消息
    public class SYJBILLHeadList
    {
        [DataMember(Order = 0)]
        public string dcmno { get; set; }//售药机编码
        [DataMember(Order = 1)]
        public string busno { get; set; }//门店编码
        [DataMember(Order = 2)]
        public string billno { get; set; }//单据号
        [DataMember(Order = 3)]
        public string callback_url { get; set; }//回调地址
        [DataMember(Order = 4)]
        public List<SYJBILLDetailList> detail { get; set; }//单据明细ID列表
    }
    public class SYJBILLDetailList
    {
        [DataMember(Order = 0)]
        public string wareid { get; set; }//商品ID
        [DataMember(Order = 1)]
        public int wareqty { get; set; }//数量
        [DataMember(Order = 2)]
        public int type { get; set; }//类型
        [DataMember(Order = 3)]
        public string makeno { get; set; }//生产批号
    }
    #endregion
    #region 售药机销售请求消息
    public class SYJSALEHeadList
    {
        [DataMember(Order = 0)]
        public string dcmno { get; set; }//售药机编码
        [DataMember(Order = 1)]
        public string busno { get; set; }//门店编码
        [DataMember(Order = 2)]
        public string billno { get; set; }//单据号
        [DataMember(Order = 3)]
        public List<SYJBILLDetailList> detail { get; set; }//单据明细ID列表
    }
    public class SYJSALEDetailList
    {
        [DataMember(Order = 0)]
        public string wareid { get; set; }//商品ID
        [DataMember(Order = 1)]
        public int wareqty { get; set; }//数量
        [DataMember(Order = 2)]
        public decimal saleprice { get; set; }//售价
        [DataMember(Order = 3)]
        public string makeno { get; set; }//生产批号
    }
    #endregion
    #region B2B返回消息明细
    public class RProductList
    {
        [DataMember(Order = 0)]
        public string erpOrderItemId { get; set; }//erp订单明细id

        [DataMember(Order = 1)]
        public string productCode { get; set; }//产品编码

        [DataMember(Order = 2)]
        public string amount { get; set; }//数量  

        [DataMember(Order = 3)]
        public string price { get; set; }//单价

        [DataMember(Order = 4)]
        public string money { get; set; }//金额

        [DataMember(Order = 5)]
        public string promoMoney { get; set; }//折扣金额为负

        [DataMember(Order = 6)]
        public string batchNumber { get; set; }//批号

        [DataMember(Order = 7)]
        public string validDate { get; set; }//有效期至

        [DataMember(Order = 8)]
        public string checkFile { get; set; }//质检单路径
    }
    #endregion
}
