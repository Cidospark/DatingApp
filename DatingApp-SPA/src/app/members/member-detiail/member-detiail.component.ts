import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detiail',
  templateUrl: './member-detiail.component.html',
  styleUrls: ['./member-detiail.component.css']
})
export class MemberDetiailComponent implements OnInit {
  user: User;
  constructor(private userService: UserService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
  }

  // loadUser() {
  //   this.userService.getUser(+this.route.snapshot.params['id'])
  //   .subscribe((user: User) => {
  //     this.user = user;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

}
