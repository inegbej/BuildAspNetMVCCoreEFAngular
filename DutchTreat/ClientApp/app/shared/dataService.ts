import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from 'rxjs/operators';
import { Observable } from "rxjs";
import { Product } from "./product";
import { Order, OrderItem } from "./order";


@Injectable()
export class DataService {

    public products = [];
    public order: Order = new Order();

    // support login
    private token: string = "";
    private tokenExpiration: Date = new Date();

    // read only property: Test to check whether login is required or not.
    public get loginRequired(): boolean {
        return this.token.length == 0 || this.tokenExpiration > new Date();
    }

    constructor(private http: HttpClient) { }

    
    loadProducts(): Observable<boolean> {
        return this.http.get("/api/products")
            .pipe(
                map((data: any[]) => {
                    this.products = data;
                    return true;
                }));
    }

    //
    public login(creds): Observable<boolean> {                                         /* using token authentication. Calls the CreateTiken API */
        return this.http.post("/account/createtoken", creds)
            .pipe(
                map((response: any) => {
                    let tokenInfo = response;
                    this.token = tokenInfo.token;
                    this.tokenExpiration = tokenInfo.expiration;
                    return true;
                }));
    }

    // 
    public checkout() {
        if (!this.order.orderNumber) {
            this.order.orderNumber = this.order.orderDate.getFullYear().toString() + this.order.orderDate.getTime().toString();
        }

        return this.http.post("/api/orders", this.order, {
            headers: new HttpHeaders({ "Authorization": "Bearer " + this.token })    // 
        })
            .pipe(
                map(response => {
                    this.order = new Order();
                    return true;
                }));
    }

    // 
    public AddToOrder(product: Product) {

        let item: OrderItem = this.order.items.find(i => i.productId == product.id);

        if (item) {

            item.quantity++;

        } else {
       
            item = new OrderItem();
            item.productId = product.id;
            item.productArtist = product.artist;
            item.productCategory = product.category;
            item.productArtId = product.artId;
            item.productTitle = product.title;
            item.productSize = product.size;
            item.unitPrice = product.price;
            item.quantity = 1;

            this.order.items.push(item);
        }
    }

}