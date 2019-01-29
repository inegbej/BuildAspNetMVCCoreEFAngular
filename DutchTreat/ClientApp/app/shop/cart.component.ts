import { Component } from "@angular/core";
import { DataService } from "../shared/dataService";
import { Router } from "@angular/router";

@Component({
    selector: "the-cart",
    templateUrl: "cart.component.html",
    styleUrls: []
})
export class Cart {

    constructor(private data: DataService, private router: Router) { }

    
    onCheckout() {                       /* NOTE: This does not rely on cookie based authentication as it will be login in to angular component itself. */
        if (this.data.loginRequired) {
            // Force Login
            this.router.navigate(["login"]);
        } else {
            // Go to checkout
            this.router.navigate(["checkout"]);
        }
    }

}