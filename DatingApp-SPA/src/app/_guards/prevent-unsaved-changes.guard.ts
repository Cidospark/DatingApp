import { Injectable } from "@angular/core";
import { CanDeactivate } from "@angular/router";
import { MemberEditComponent } from "../members/member-edit/member-edit.component";

@Injectable()

export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent>{

    canDeactivate(component: MemberEditComponent) {
        if (component.editFOrm.dirty){
            return confirm('Unsaved changes will be lost. Still want to continue?')
        }
        return true;
    }

}