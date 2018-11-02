import { Component } from '@angular/core';
import { Http } from "@angular/http";

class ResultModel {
  public isSuccess: boolean;
  public message: string;
  public data: any;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private api: string = "/api/authentication";
  private captchaUrl1: string = `${this.api}/captcha1`;
  private captchaUrl2: string = `${this.api}/captcha2`;
  title: string = "Login";
  isLogin: boolean;
  message: string;
  username: string;
  password: string;
  code: string;

  constructor(private http: Http) {
    this.loadUserInfo();
  }

  loadUserInfo(): void {
    this.http.get(this.api).subscribe(
      (response) => {
        let result: ResultModel = response.json();
        if (!result.isSuccess) {
          this.showMessage(result.message);
        } else if (result.data) {
          this.username = result.data as string;
          this.isLogin = true;
        } else {
          this.clear();
        }
      });
  }
  
  login(): void {
    this.clearMessage();
    this.http.post(this.api,
      {
        username: this.username,
        password: this.password,
        code: this.code
      }).subscribe(
        (response) => {
          let result: ResultModel = response.json();
          if (!result.isSuccess) {
            this.showMessage(result.message);
            this.resetCaptcha();
          } else {
            this.showMessage(`Login successfully`);
            this.isLogin = true;
          }
        });
  }

  logout(): void {
    this.http.delete(this.api).subscribe(
      (response) => {
        let result: ResultModel = response.json();
        if (!result.isSuccess) {
          this.showMessage(result.message);
        } else {
          this.clear();
        }
      });
  }

  resetCaptcha(): void {
    this.code = "";
    this.captchaUrl1 = `${this.api}/captcha1?${Date.now()}`;
    this.captchaUrl2 = `${this.api}/captcha2?${Date.now()}`;
  }

  clear(): void {
    this.isLogin = false;
    this.username = "";
    this.password = "";
    this.resetCaptcha();
    this.clearMessage();
  }

  clearMessage(): void {
    this.message = "";
  }

  showMessage(message: string): void {
    this.message = message;
  }

}
