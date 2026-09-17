import {injectable} from "inversify";
import {IUsernameService} from "@/spi/AuthSPI";

@injectable()
export default class UsernameService implements IUsernameService{
    private username: string = 'anonymous';
    
    public setUsername(username: string){
        this.username = username;    
    };
    
    public getUsername() {
        return this.username;
    }
}