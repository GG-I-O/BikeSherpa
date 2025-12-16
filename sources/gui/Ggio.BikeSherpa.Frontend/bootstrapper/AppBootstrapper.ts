// import Interceptors from "./interceptors/Interceptors";
import axios from "axios";
import IOCContainerBootstrapper from "./IOCContainerBootstrapper";
import InterceptorBootstrap from "./interceptors/InterceptorBootstrap";

export default class AppBootstrapper {

    public static init() {
        IOCContainerBootstrapper.init();

        axios.defaults.baseURL = process.env.EXPO_PUBLIC_API_URL;

        const interceptors = new InterceptorBootstrap();
        interceptors.startToIntercept();
    }
}