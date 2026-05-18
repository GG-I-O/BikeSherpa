import { IAuthService } from "@/spi/AuthSPI";
import { injectable } from "inversify";
import { Credentials } from "react-native-auth0/lib/typescript/src/core/models";
import DispatcherRole from "@/infra/auth/dispatcherRole";
import { Buffer } from 'buffer';

@injectable()
export default class AuthService implements IAuthService {
    private getCredentials?: (scope?: string | undefined, minTtl?: number | undefined, parameters?: Record<string, unknown> | undefined, forceRefresh?: boolean) => Promise<Credentials>
    private readonly audience = process.env.EXPO_PUBLIC_AUTH_AUDIENCE;
    private readonly scope = process.env.EXPO_PUBLIC_AUTH_DEV_SCOPE;

    public setCredentialMethod(method: (params: any) => Promise<any>): void {
        this.getCredentials = method;
    }

    public async getToken(): Promise<string | null> {
        if (!this.getCredentials || !this.audience)
            throw new Error("AuthService not initialized");
        const credentials = await this.getCredentials(this.scope, undefined, { audience: this.audience });
        return credentials.accessToken;
    }
    
    public async isDispatcher(): Promise<boolean> {
        if (!this.getCredentials || !this.audience)
            throw new Error("AuthService not initialized");
        const credentials = await this.getCredentials(this.scope, undefined, { audience: this.audience });
        const payloadBase64 = credentials.accessToken.split('.')[1]
            .replace(/-/g, '+')
            .replace(/_/g, '/');

        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);
        
        return Array.isArray(payload.roles_claim) && payload.roles_claim.includes(DispatcherRole);
    }
}