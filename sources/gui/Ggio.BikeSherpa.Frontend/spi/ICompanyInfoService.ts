export interface ICompanyInfoService {
    getCompanyLabel(): Promise<string>;

    getCompanyPhone(): Promise<string | undefined>;
}