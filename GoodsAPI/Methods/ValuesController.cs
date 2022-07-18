using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoodsAPI.Data;
using GoodsAPI.Models;
using System.Text.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GoodsAPI.Methods
{
    
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly GoodsContext _context;

        public ValuesController(GoodsContext context)
        {
            _context = context;
        }
       
        //test function, can be ignored.
        [HttpPost]
        [Route("test")]
        public string test(string? column, string? value, int? page)
        {

            var BP = _context.BusinessPartners.Include(b => b.Btype).ToList();


            //  return BP[0].Btype.TypeName;
            return "test";
            

        }

        //document class
        public class Document
        {
            public string? type { get; set; }
            public string? bpcode { get; set; }

            public string? itemcode { get; set; }

            public decimal? quantity { get; set; }

            public string? comment { get; set; }

            public int? userId    { get; set; }

            public int? docId { get; set; }

        }




        //read items gets data via parameters in the url

        [HttpPost]
        [Route("readitems")]
        public async Task<ActionResult<IEnumerable<Items>>> ReadItem(string column, string value, int? page )
        {
           //Changing the amount of items per page can easily be changed by adding itemsperpage as a parameter or changing below
            int itemsperpage = 5;
            int pagenum = page ==null ?  1 : page.Value;
            int maxpages;
           
            if ( column == null)
            {
                return BadRequest("Please input a column.");
            }
            if (value == null)
            {
                return BadRequest("Please input data to filter by.");
            }
            else
            {
                var items =   from item in _context.Items select item;
               //calculate max page.
                maxpages = (int)Math.Ceiling(items.Count()/ (double)itemsperpage);
                //small about of columns so I went with an easily upgradable switch
                switch (column)
                {
                    case "ItemCode":
                        //Warn no Orderby, but wasn't specified in project so I left it as is.
                        items = items.Where(x => x.ItemsCode == null ? false : x.ItemsCode.Contains(value) ).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);
                
                                                                           
                        break;
                    case "ItemName":
                                                                            
                        items = items.Where(x => x.ItemName == null ? false : x.ItemName.Contains(value)).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);
                                                                            
                        break;
                    case "Active":
                        if(value == "1" || value == "true")
                            items =  items.Where(x => x.Active == true).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);
                           
                        if (value == "0" || value == "false")
                            items = items.Where(x => x.Active == false).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);
                        break;
                    default:
                        return BadRequest("No column like that exists.");
                }
                //added max page to the header!
                Response.Headers.Add("Max-Pages", JsonSerializer.Serialize(maxpages));
                return await items.ToListAsync();
            }
            
        }
        //very similar in execution to ReadItems
        [HttpPost]
        [Route("read_business_partners")]
        public async Task<ActionResult<IEnumerable<BusinessPartners>>> ReadBusinessPartners(string column, string value, int? page)
        {
            //Changing the amount of items per page can easily be changed by adding itemsperpage as a parameter or changing below
            int itemsperpage = 5;
            int pagenum = page == null ? 1 : page.Value;
            int maxpages;
           
            if (column == null)
            {
                return BadRequest("Please input a column.");
            }
            if (value == null)
            {
                return BadRequest("Please input data to filter by.");
            }
            else
            {
                var BP = from b in _context.BusinessPartners select b;

                maxpages = (int)Math.Ceiling(BP.Count() / (double)itemsperpage);

                switch (column)
                {
                    case "BPCode":
                        //Warn no Orderby, but wasn't specified in project so I left it as is.
                        BP = BP.Where(x => x.BPCode == null ? false : x.BPCode.Contains(value)).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);


                        break;
                    case "BPName":

                        BP = BP.Where(x => x.BPName == null ? false : x.BPName.Contains(value)).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);

                        break;
                    case "BPType":

                        BP = BP.Where(x => x.Btype == null ? false : x.Btype.TypeCode == value[0]).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);

                        break;
                    case "Active":
                        if (value == "1" || value == "true")
                            BP = BP.Where(x => x.Active == true).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);

                        if (value == "0" || value == "false")
                            BP = BP.Where(x => x.Active == false).Skip((pagenum - 1) * itemsperpage).Take(itemsperpage);
                        break;
                    default:
                        return BadRequest("No column like that exists.");
                }
                Response.Headers.Add("Max-Pages", JsonSerializer.Serialize(maxpages));
                return await BP.ToListAsync();
            }

        }

        [HttpPost]
        [Route("add_document")]
        public async Task<ActionResult<IEnumerable<object>>> AddDocument([FromBody] Document doc)
        {
            if(doc == null)
            {
                return BadRequest("Document Empty");
            }
            if(doc.type == null || doc.bpcode==null || doc.quantity==null || doc.itemcode==null || doc.userId == null)
            {
                return BadRequest("Document missing data");
            }

           



           //switch because only data types
            switch (doc.type)
            {
                case "PurchaseOrders":
                    //validate that bp exists,is active and doesnt have S
                    if ((from b in _context.BusinessPartners where b.BPCode == doc.bpcode && b.Active == true && b.BPType != 'S' select b).Any())
                    {
                        //validate Item exists and is active
                        if ((from i in _context.Items where i.ItemsCode == doc.itemcode && i.Active == true select i ).Any())
                        {
                            //creating PO from doc to add to db then using id from PO for POL 
                            PurchaseOrders PO = new PurchaseOrders { BPCode = doc.bpcode, CreateDate = DateTime.Now, CreatedBy = doc.userId };
                           await _context.PurchaseOrders.AddAsync(PO);
                           await _context.SaveChangesAsync();
                            PurchaseOrdersLines POL = new PurchaseOrdersLines { DocID = PO.Id, ItemCode = doc.itemcode, Quantity = doc.quantity };
                            await _context.PurchaseOrdersLines.AddAsync(POL);
                            await _context.SaveChangesAsync();
                            return Content("Document created");
                            
                        }
                        else { return BadRequest("Item not Active");}
                    }
                    else {return BadRequest("Not possible to add that Business Partner");}
                        
                    
                   
                    //similar to PO
                case "SaleOrders":
                    if ((from b in _context.BusinessPartners where b.BPCode == doc.bpcode && b.Active == true && b.BPType != 'V' select b).Any())
                    {
                        SaleOrders SO = new SaleOrders { BPCode = doc.bpcode, Createdate = DateTime.Now, CreatedBy = doc.userId };
                        await _context.SaleOrders.AddAsync(SO);
                        await _context.SaveChangesAsync();
                        SaleOrdersLines SOL = new SaleOrdersLines { DocID = SO.Id, ItemCode = doc.itemcode, Quantity = doc.quantity };
                        await _context.SaleOrdersLines.AddAsync(SOL);
                        await _context.SaveChangesAsync();
                        string comment = doc.comment ==null ? "" : doc.comment;
                        await _context.SaleOrdersLinesComments.AddAsync( new SaleOrdersLinesComments { Comment = comment,DocID=SO.Id,LineID= SOL.LineID });
                        await _context.SaveChangesAsync();

                        return Content("Document created") ;
                    }
                    else { return BadRequest("Not possible to add that Business Partner"); }
                   
                default:
                    return BadRequest("Error: Document type doesnt exist");
                   



            }


        }
        //POST: update document with parameter document taken from body
        [HttpPost]
        [Route("update_document")]
        public async Task<ActionResult<IEnumerable<object>>> UpdateDocument([FromBody] Document doc)
        {
            int id = -1;
            if (doc == null)
            {
                return BadRequest("Document Empty");
            }
            if(doc.docId==null)
            {
                return BadRequest("No ID Given");
            }
            else
            {
                id = doc.docId.Value;
            }
            if (doc.type == null || doc.bpcode == null || doc.quantity == null || doc.itemcode == null || doc.userId == null)
            {
                return BadRequest("Document missing data");
            }

           



            //only two types so used switch
            switch (doc.type)
            {
                case "PurchaseOrders":
                    //validation
                    if ((from b in _context.BusinessPartners where b.BPCode == doc.bpcode && b.Active == true && b.BPType != 'S' select b).Any())
                    {
                        //validation
                        if ((from i in _context.Items where i.ItemsCode == doc.itemcode && i.Active == true select i).Any())
                        {

                            //using where and include to obtain data with relationship
                            var POL = await _context.PurchaseOrdersLines.Where(x => x.LineID == id).Include(x => x.purchaseOrders).ToListAsync() ;
                            if (POL.Any())
                            {
                              return BadRequest("No Document with that ID");
                            }
                            //updating instance of data
                            POL[0].ItemCode = doc.itemcode;
                            POL[0].Quantity = doc.quantity;
                            if (POL[0].purchaseOrders == null)
                            {
                               return BadRequest("Document error");
                            }
                            //checked above for null safety  ***********************
                                                                                                 #pragma warning disable CS8602 // Dereference of a possibly null reference.
                            POL[0].purchaseOrders.BPCode = doc.bpcode;
                            POL[0].purchaseOrders.LastUpdateDate = DateTime.Now;
                            POL[0].purchaseOrders.LastUpdatedBy = doc.userId;
                                                                                                 #pragma warning restore CS8602 // Dereference of a possibly null reference.
                            _context.PurchaseOrdersLines.Update(POL[0]);
                                                                                                #pragma warning disable CS8604 // Possible null reference argument.
                            _context.PurchaseOrders.Update(POL[0].purchaseOrders);
                                                                                                #pragma warning restore CS8604 // Possible null reference argument.

                            await _context.SaveChangesAsync();
                            return Content("Document created");

                        }
                        else { return BadRequest("Item not Active"); }
                    }
                    else { return BadRequest("Not possible to add that Business Partner"); }



                //similar to PO
                case "SaleOrders":
                    if ((from b in _context.BusinessPartners where b.BPCode == doc.bpcode && b.Active == true && b.BPType != 'V' select b).Any())
                    {
                        var SOL = await _context.SaleOrdersLines.Where(x => x.LineID == id).Include(x => x.SaleOrders).ToListAsync();
                        if (SOL.Any())
                        {
                            return BadRequest("No Document with that ID");
                        }

                        SOL[0].ItemCode = doc.itemcode;
                        SOL[0].Quantity = doc.quantity;
                        if (SOL[0].SaleOrders == null)
                        {
                            return BadRequest("Document error");
                        }
                        //checked above for null safety  ***********************
                                                                                        #pragma warning disable CS8602 // Dereference of a possibly null reference.
                        SOL[0].SaleOrders.BPCode = doc.bpcode;
                        SOL[0].SaleOrders.LastUpdatedate = DateTime.Now;
                        SOL[0].SaleOrders.LastUpdatedBy = doc.userId;
                                                                                        #pragma warning restore CS8602 // Dereference of a possibly null reference.
                        _context.SaleOrdersLines.Update(SOL[0]);
                                                                                        #pragma warning disable CS8604 // Possible null reference argument.
                        _context.SaleOrders.Update(SOL[0].SaleOrders);
                                                                                        #pragma warning restore CS8604 // Possible null reference argument.

                        if (doc.comment != null) { 
                            var SC = await _context.SaleOrdersLinesComments.Where(x => x.LineID == id).ToListAsync();
                            if (SC.Any())
                            {
                                return BadRequest("No Comment Document with that ID");
                            }
                            SC[0].Comment = doc.comment;
                            _context.SaleOrdersLinesComments.Update(SC[0]);
                        }
                        await _context.SaveChangesAsync();
                        return Content("Document created");

                      

                    }
                    else { return BadRequest("Not possible to add that Business Partner"); }

                default:
                    return BadRequest("Error: Document type doesnt exist");




            }
        }

        //Get: recives as parameter in url id and type and deletes relivent data
        [HttpGet]
        [Route("delete_document")]
        public async Task<IActionResult> deleteDocument(int? id,string? type)
        {
            if (id==null || type==null)
            {
                return BadRequest("Missing Data");
            }
            //switch cause only two types
            switch (type)
            {
                case "PurchaseOrders":
                    //finds via id
                    var POL = await _context.PurchaseOrdersLines.FindAsync(id);
                    //will delete the rest of the doc via cascade.
                    if (POL != null)
                    {
                        //removes POL and others via cascade
                        _context.PurchaseOrdersLines.Remove(POL);
                        await _context.SaveChangesAsync();

                        return NoContent();
                    }
                    else
                    {
                        return NotFound("Document not found");
                    }
                    
                //similar to PO
                case "SalesOrders":
                    var SOLC = await _context.SaleOrdersLinesComments.FindAsync(id);
                    //will delete the rest of the doc via cascade.
                    if (SOLC != null)
                    {
                        _context.SaleOrdersLinesComments.Remove(SOLC);
                        await _context.SaveChangesAsync();

                        return NoContent();
                    }
                    else
                    {
                        return NotFound("Document not found");
                    }
                 default: return BadRequest("Data Type Invalid");
            }
        }

        //GET: recives id and type via url for corresponding document to obtain 
        [HttpGet]
        [Route("get_document")]
        public async Task<ActionResult<IEnumerable<Object>>> GetDocument(int? id, string? type)
        {
            if (id == null || type == null)
            {
                return BadRequest("Missing Data");
            }

            switch (type)
            {
                case "PurchaseOrders":
                    //chaining includes and theninclude to access via relationship
                                                                                                                                                #pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var POL = await _context.PurchaseOrdersLines.Where(x => x.LineID == id).Include(x => x.purchaseOrders).ThenInclude(x => x.BP)
                        .Include(x => x.purchaseOrders).ThenInclude(x => x.Users1).Include(x => x.purchaseOrders).ThenInclude(x => x.Users2).ToListAsync();

                    if (POL.Any())
                    {
                        return POL;
                    }
                    else
                    {
                        return NotFound("Data not found");
                    }
           
                   
                


                case "SalesOrders":
                    //chaining includes and theninclude to access via relationship
                    var SOLC = await _context.SaleOrdersLinesComments.Where(x => x.LineID == id).Include(x => x.SaleOrders).ThenInclude(x => x.BP)
                        .Include(x => x.SaleOrders).ThenInclude(x => x.Users1).Include(x => x.SaleOrders).ThenInclude(x => x.Users2).Include(x=>x.SaleOrdersLines).ToListAsync();
                                                                                                                                                            #pragma warning restore CS8602 // Dereference of a possibly null reference.
                    if (SOLC.Any())
                    {
                        return SOLC;
                    }
                    else
                    {
                        return NotFound("Data not found");
                    }
                default: return BadRequest("Data Type Invalid");
            }
        }
    }
   
}
