import {Container} from "inversify";
import {IAuthService, IUsernameService, IUserService} from "@/spi/AuthSPI";
import {UserService} from "@/infra/auth/UserService";
import {AuthServiceIdentifier} from "@/infra/auth/bootstrapper/AuthServiceIdentifier";
import AuthService from "@/infra/auth/AuthService";
import UsernameService from "@/infra/auth/UsernameService";

export default class AuthBootstrapper {
    public static init(IOCContainer: Container) {
        IOCContainer.bind<IUserService>(AuthServiceIdentifier.UserService).to(UserService).inSingletonScope();

        IOCContainer.bind<IAuthService>(AuthServiceIdentifier.AuthService).to(AuthService).inSingletonScope();
        
        IOCContainer.bind<IUsernameService>(AuthServiceIdentifier.UsernameService).to(UsernameService).inSingletonScope();
    }
}