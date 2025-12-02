// import Interceptors from "./interceptors/Interceptors";
import IOCContainerBootstrapper from "./IOCContainerBootstrapper";

export default class AppBootstrapper {

    public static init() {
        IOCContainerBootstrapper.init();

        // const interceptors = new Interceptors();
        // interceptors.startToIntercept();
    }
}